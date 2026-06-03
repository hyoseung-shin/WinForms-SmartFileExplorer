// ==========================================
// 자동 정리(분류·폴더 생성·이동) 처리 Service 클래스
//
// 주요 기능:
// - 확장자/파일 유형 기반 자동 분류
//   (FileCategoryHelper.GetCategory 재사용)
// - 카테고리 폴더 자동 생성
//   (대상 폴더가 없으면 생성, 이미 있으면 그대로 사용)
// - 파일 이동 처리 로직
//   (이름 충돌 시 "이름 (1).확장자" 형태로 안전 이름 부여)
//
// 사용 흐름:
//   1) BuildPlan(): 파일 목록 → 이동 계획(OrganizePlanItem 리스트) 생성
//   2) Execute(): 계획대로 폴더 생성 + 파일 이동 수행
//   3) Result 반환: 성공/실패 개수, 카테고리별 처리 개수 등
// ==========================================

using System; // Exception, StringComparison 사용
using System.Collections.Generic; // List, Dictionary 사용
using System.IO; // File, Directory, Path, FileInfo 사용
using System.Linq; // GroupBy, Count, ToDictionary 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 자동 정리 계획 한 줄(파일 1개에 대한 이동 정보)을 표현하는 클래스
    public class OrganizePlanItem
    {
        // 원본 파일 정보
        public FileInfo Source;

        // 분류된 카테고리 이름 (예: "이미지", "동영상", "문서", "기타")
        public string CategoryName;

        // 이동될 대상 폴더 경로 (예: "C:\Users\xxx\Downloads\이미지")
        public string TargetFolderPath;

        // 이동 후 최종 파일 전체 경로 (충돌 시 자동 변경됨)
        public string TargetFullPath;

        // 이름 충돌로 인해 파일명이 변경된 경우 true
        public bool IsRenamed;
    }

    // 한 번 수행된 파일 이동 1건을 표현 (되돌리기에 사용)
    public class MoveRecord
    {
        // 이동 전 원본 경로
        public string OriginalPath;

        // 이동 후 새 경로
        public string MovedPath;

        // 이동 시 새로 생성된 카테고리 폴더 (없으면 null)
        // 되돌리기 시 비어 있는 폴더가 되면 같이 제거하기 위함
        public string CreatedFolder;
    }

    // 자동 정리 실행 결과를 담는 클래스
    public class AutoOrganizeResult
    {
        // 이동 성공 파일 개수
        public int SuccessCount;

        // 이동 실패 파일 개수
        public int FailedCount;

        // 건너뛴 파일 개수(이미 대상 폴더와 동일한 위치에 있을 때 등)
        public int SkippedCount;

        // 카테고리별 성공 처리 개수 (예: 이미지=5, 문서=12)
        public Dictionary<string, int> CategoryCounts = new Dictionary<string, int>();

        // 실패 발생 시의 오류 메시지 모음
        public List<string> Errors = new List<string>();

        // 새로 만들어진 폴더 경로 모음(중복 없이 저장)
        public List<string> CreatedFolders = new List<string>();

        // 되돌리기를 위한 실제 이동 내역 (성공한 건만 기록)
        public List<MoveRecord> ExecutedMoves = new List<MoveRecord>();

        // 정리 실행 시각 (사용자에게 "방금 전 / 5분 전" 같은 표시용)
        public DateTime ExecutedAt = DateTime.Now;
    }

    // 자동 정리 처리를 담당하는 Service 클래스
    public static class AutoOrganizeService
    {
        // -----------------------------------
        // 1) 이동 계획 생성
        //    파일 목록 → 카테고리 분류 → OrganizePlanItem 리스트
        // -----------------------------------
        public static List<OrganizePlanItem> BuildPlan(
            List<FileInfo> files, // 정리 대상 파일 목록
            string baseFolder) // 기준 폴더(예: 다운로드 폴더)
        {
            // 반환할 계획 목록
            List<OrganizePlanItem> plan = new List<OrganizePlanItem>();

            // 입력값이 비어있으면 빈 계획 반환
            if (files == null || files.Count == 0 || string.IsNullOrEmpty(baseFolder))
                return plan;

            // 카테고리 폴더에 이미 있는 파일은 자동 정리 대상에서 제외하기 위한 비교용 폴더 집합
            // (예: "이미지", "동영상" 등 이미 만들어 둔 분류 폴더의 절대 경로)
            HashSet<string> categoryFolderSet = BuildCategoryFolderSet(baseFolder);

            // 각 파일에 대해 카테고리 분류 및 대상 경로 계산
            foreach (FileInfo fi in files)
            {
                // 잘못된 항목이거나 실제 파일이 아니면 건너뜀
                if (fi == null || !fi.Exists)
                    continue;

                // 부모 폴더 경로 (대소문자 차이를 줄이기 위해 풀 경로로 비교)
                string parentFolder = Path.GetFullPath(fi.DirectoryName);

                // 이미 카테고리 폴더 안에 들어있는 파일은 정리 대상에서 제외
                // (예: "다운로드\이미지\photo.jpg"는 이미 분류되어 있으므로 건너뜀)
                if (categoryFolderSet.Contains(parentFolder))
                    continue;

                // 확장자 기반 카테고리 판정 (이미지/동영상/문서/압축파일/실행파일/기타)
                string category = FileCategoryHelper.GetCategory(fi);

                // 대상 폴더 전체 경로 생성 (예: "C:\...\Downloads\이미지")
                string targetFolder = Path.Combine(baseFolder, category);

                // 이동 후 1차 후보 경로 (충돌이 없다면 이 경로로 이동)
                string candidatePath = Path.Combine(targetFolder, fi.Name);

                // 이름 충돌 시 안전 이름 생성(같은 폴더에 동일 이름이 있으면 "(1)", "(2)" 추가)
                string finalPath = ResolveCollisionName(targetFolder, fi.Name, plan);

                // 이름이 바뀌었는지 여부
                bool renamed = !string.Equals(finalPath, candidatePath, StringComparison.OrdinalIgnoreCase);

                // 계획 항목 생성
                OrganizePlanItem item = new OrganizePlanItem
                {
                    Source = fi,
                    CategoryName = category,
                    TargetFolderPath = targetFolder,
                    TargetFullPath = finalPath,
                    IsRenamed = renamed
                };

                // 계획 목록에 추가
                plan.Add(item);
            }

            // 카테고리 → 이름 순으로 정렬해서 반환(사용자가 보기 좋게)
            return plan
                .OrderBy(p => p.CategoryName)
                .ThenBy(p => p.Source.Name)
                .ToList();
        }

        // -----------------------------------
        // 2) 계획 실행
        //    폴더가 없으면 자동 생성 + 파일을 카테고리 폴더로 이동
        // -----------------------------------
        public static AutoOrganizeResult Execute(List<OrganizePlanItem> plan)
        {
            // 결과 객체 초기화
            AutoOrganizeResult result = new AutoOrganizeResult();

            // 계획이 비어있으면 그대로 반환
            if (plan == null || plan.Count == 0)
                return result;

            // 새로 만든 폴더를 중복 없이 추적하기 위한 집합
            HashSet<string> createdSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 계획 한 줄씩 처리
            foreach (OrganizePlanItem item in plan)
            {
                try
                {
                    // 원본 파일이 사라졌으면 건너뜀
                    if (item.Source == null || !File.Exists(item.Source.FullName))
                    {
                        result.SkippedCount++;
                        continue;
                    }

                    // 원본 파일과 이동 대상 경로가 동일하면(이미 분류된 상태) 건너뜀
                    if (string.Equals(
                            Path.GetFullPath(item.Source.FullName),
                            Path.GetFullPath(item.TargetFullPath),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        result.SkippedCount++;
                        continue;
                    }

                    // 대상 폴더가 없으면 새로 만들기 (자동 폴더 생성 기능)
                    bool createdNow = false;
                    if (!Directory.Exists(item.TargetFolderPath))
                    {
                        Directory.CreateDirectory(item.TargetFolderPath);

                        // 새로 만든 폴더로 기록
                        if (createdSet.Add(item.TargetFolderPath))
                        {
                            result.CreatedFolders.Add(item.TargetFolderPath);
                            createdNow = true;
                        }
                    }

                    // 원본 경로 백업(이동 후에는 FullName이 바뀌므로 미리 저장)
                    string originalPath = item.Source.FullName;

                    // 파일 이동(실제 이동 처리)
                    File.Move(originalPath, item.TargetFullPath);

                    // 되돌리기를 위한 이동 내역 기록
                    result.ExecutedMoves.Add(new MoveRecord
                    {
                        OriginalPath = originalPath,
                        MovedPath = item.TargetFullPath,
                        CreatedFolder = createdNow ? item.TargetFolderPath : null
                    });

                    // 성공 카운트 증가
                    result.SuccessCount++;

                    // 카테고리별 카운트 증가
                    if (result.CategoryCounts.ContainsKey(item.CategoryName))
                        result.CategoryCounts[item.CategoryName]++;
                    else
                        result.CategoryCounts[item.CategoryName] = 1;
                }
                catch (Exception ex)
                {
                    // 실패 카운트 증가 및 오류 메시지 기록
                    result.FailedCount++;
                    result.Errors.Add(item.Source.Name + " → " + ex.Message);
                }
            }

            // 최종 결과 반환
            return result;
        }

        // -----------------------------------
        // 3) 원스텝 실행(계획 → 즉시 실행)
        //    Form 측에서 미리보기를 띄우지 않고 바로 정리하고 싶을 때 사용
        // -----------------------------------
        public static AutoOrganizeResult OrganizeFolder(string baseFolder, List<FileInfo> files)
        {
            // 계획 생성
            List<OrganizePlanItem> plan = BuildPlan(files, baseFolder);

            // 계획 실행
            return Execute(plan);
        }

        // -----------------------------------
        // (Helper) 이름 충돌 안전 이름 생성
        //   - 대상 폴더에 이미 동일 이름 파일이 있으면
        //     "이름 (1).확장자", "이름 (2).확장자" 형태로 번호를 붙여 안전 경로 반환
        //   - 같은 계획 안에서 동시에 이동되는 다른 파일과의 충돌도 함께 검사
        // -----------------------------------
        private static string ResolveCollisionName(
            string targetFolder, // 이동할 폴더 경로
            string fileName, // 원본 파일 이름(확장자 포함)
            List<OrganizePlanItem> existingPlan) // 이미 만들어진 계획(같은 배치 충돌 방지용)
        {
            // 1차 후보 전체 경로
            string candidate = Path.Combine(targetFolder, fileName);

            // 디스크에도 없고, 계획 안에서도 중복이 없다면 그대로 반환
            if (!File.Exists(candidate) && !PlanContainsTarget(existingPlan, candidate))
                return candidate;

            // 충돌이 있으면 번호를 붙여서 새 이름 시도
            string nameOnly = Path.GetFileNameWithoutExtension(fileName); // 확장자를 뺀 이름
            string ext = Path.GetExtension(fileName); // 확장자(.포함)

            // 1부터 시작해서 사용 가능한 번호를 찾음
            int n = 1;
            while (true)
            {
                // " (n)" 형태로 번호 부여
                string newName = nameOnly + " (" + n + ")" + ext;
                string newPath = Path.Combine(targetFolder, newName);

                // 디스크 및 같은 배치 계획 양쪽 모두 충돌이 없을 때 채택
                if (!File.Exists(newPath) && !PlanContainsTarget(existingPlan, newPath))
                    return newPath;

                // 다음 번호 시도
                n++;

                // 안전장치: 너무 많은 시도(이론상 발생 불가지만 무한루프 방지)
                if (n > 100000)
                    return newPath;
            }
        }

        // -----------------------------------
        // (Helper) 같은 배치 계획 안에서 동일한 TargetFullPath가 있는지 검사
        //   - 같은 배치에 같은 이름 파일 2개가 있을 때
        //     두 번째 파일이 첫 번째 파일을 덮어쓰지 않도록 막기 위함
        // -----------------------------------
        private static bool PlanContainsTarget(List<OrganizePlanItem> existingPlan, string path)
        {
            // 계획이 없으면 충돌 없음
            if (existingPlan == null || existingPlan.Count == 0)
                return false;

            // 대소문자 구분 없이 비교 (Windows 환경 기준)
            return existingPlan.Any(p =>
                string.Equals(p.TargetFullPath, path, StringComparison.OrdinalIgnoreCase));
        }

        // -----------------------------------
        // (Helper) 카테고리 폴더 경로 집합 생성
        //   - 이미 만들어둔 분류 폴더("이미지", "동영상" 등) 안의 파일은
        //     자동 정리 대상에서 제외하기 위함
        // -----------------------------------
        private static HashSet<string> BuildCategoryFolderSet(string baseFolder)
        {
            // Windows 경로는 대소문자 구분 없이 비교
            HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 등록된 카테고리 이름 목록 가져오기
            foreach (string cat in FileCategoryHelper.GetAllCategories())
            {
                // 카테고리 폴더의 절대 경로를 집합에 추가
                set.Add(Path.GetFullPath(Path.Combine(baseFolder, cat)));
            }

            // 집합 반환
            return set;
        }

        // -----------------------------------
        // (Helper) 계획 요약 텍스트 생성
        //   - 미리보기 메시지 등에서 사용
        //   - 예: "이미지 5개\n문서 12개\n동영상 3개"
        // -----------------------------------
        public static string SummarizePlan(List<OrganizePlanItem> plan)
        {
            // 계획이 비어있으면 안내문 반환
            if (plan == null || plan.Count == 0)
                return "정리할 파일이 없습니다.";

            // 카테고리별 개수 집계
            var grouped = plan
                .GroupBy(p => p.CategoryName)
                .OrderBy(g => g.Key)
                .ToList();

            // 각 라인을 만들어 합치기
            List<string> lines = new List<string>();

            foreach (var g in grouped)
                lines.Add(" • " + g.Key + ": " + g.Count() + "개");

            // 합계 라인 추가
            lines.Add("");
            lines.Add(" 합계: " + plan.Count + "개 파일");

            // 줄바꿈으로 합쳐 반환
            return string.Join("\n", lines);
        }

        // -----------------------------------
        // 4) 되돌리기 (Undo)
        //    - 이전 Execute()가 기록한 ExecutedMoves를 역순으로 되돌림
        //    - 파일을 원래 위치로 복귀시키고,
        //      비어 있는 카테고리 폴더는 함께 제거
        //    - 한 항목이 실패해도 나머지는 계속 처리(개별 격리)
        // -----------------------------------
        public static AutoOrganizeResult Undo(AutoOrganizeResult original)
        {
            // 되돌리기 결과를 별도의 Result에 담아 반환
            AutoOrganizeResult undoResult = new AutoOrganizeResult();

            // 원본이 없거나 이동 내역이 없으면 즉시 반환
            if (original == null || original.ExecutedMoves == null
                || original.ExecutedMoves.Count == 0)
                return undoResult;

            // 마지막에 만든 폴더부터 정리하기 위해 역순으로 처리
            for (int i = original.ExecutedMoves.Count - 1; i >= 0; i--)
            {
                MoveRecord rec = original.ExecutedMoves[i];

                try
                {
                    // 이동된 파일이 사라졌으면(사용자가 직접 또 옮긴 경우 등) 스킵
                    if (!File.Exists(rec.MovedPath))
                    {
                        undoResult.SkippedCount++;
                        continue;
                    }

                    // 원래 위치에 이미 동일 이름 파일이 있으면 충돌
                    // → " (복원 N).확장자" 형태로 안전한 이름 만들어 복귀
                    string restorePath = rec.OriginalPath;
                    if (File.Exists(restorePath))
                    {
                        string dir = Path.GetDirectoryName(restorePath);
                        string nameOnly = Path.GetFileNameWithoutExtension(restorePath);
                        string ext = Path.GetExtension(restorePath);

                        int n = 1;
                        while (File.Exists(restorePath))
                        {
                            restorePath = Path.Combine(dir, nameOnly + " (복원 " + n + ")" + ext);
                            n++;
                            if (n > 100000) break;
                        }
                    }

                    // 원래 폴더가 사라졌으면 다시 만들어 줌
                    string targetDir = Path.GetDirectoryName(restorePath);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    // 파일을 원래 위치로 되돌리기
                    File.Move(rec.MovedPath, restorePath);

                    // 되돌리기 성공 카운트
                    undoResult.SuccessCount++;

                    // 이번 이동으로 새로 만들었던 카테고리 폴더가
                    // 이제 비어 있으면 같이 삭제(폴더 정리)
                    if (!string.IsNullOrEmpty(rec.CreatedFolder)
                        && Directory.Exists(rec.CreatedFolder))
                    {
                        bool isEmpty = Directory.GetFiles(rec.CreatedFolder).Length == 0
                            && Directory.GetDirectories(rec.CreatedFolder).Length == 0;

                        if (isEmpty)
                        {
                            try { Directory.Delete(rec.CreatedFolder); } catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 되돌리기 실패 시 카운트 및 오류 기록
                    undoResult.FailedCount++;
                    undoResult.Errors.Add(Path.GetFileName(rec.MovedPath) + " → " + ex.Message);
                }
            }

            return undoResult;
        }
    }
}
