// ==========================================
// 실제 파일 작업을 담당하는 Service 클래스
//
// 주요 기능:
// - 다운로드 폴더 경로 가져오기
// - 파일 목록 불러오기
// - 파일 열기
// - 파일 삭제
// - 파일 복사
// - 파일 이동
// - 파일 이름 변경
// ==========================================

using System; // Environment 사용
using System.Collections.Generic; // List 사용
using System.Diagnostics; // Process.Start 사용
using System.IO; // File, DirectoryInfo, Path 사용
using System.Linq; // OrderByDescending, ToList 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 실제 파일 작업을 담당하는 Service 클래스
    public static class FileService
    {
        // 윈도우 다운로드 폴더 경로를 가져오는 함수
        public static string GetDownloadsPath()
        {
            // 사용자 폴더 안의 Downloads 경로 생성
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), // 사용자 폴더 경로
                "Downloads" // 다운로드 폴더 이름
            );

            // Downloads 폴더가 있으면 해당 경로 반환, 없으면 바탕화면 경로 반환
            return Directory.Exists(path)
                ? path
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        // 특정 폴더 안의 파일 목록을 불러오는 함수
        public static List<FileInfo> LoadFiles(string folderPath)
        {
            // 폴더 정보를 DirectoryInfo로 생성하고 파일 목록을 가져옴
            return new DirectoryInfo(folderPath)
                .GetFiles() // 폴더 안 파일들 가져오기
                .OrderByDescending(f => f.LastWriteTime) // 최근 수정된 파일이 위로 오게 정렬
                .ToList(); // List<FileInfo>로 변환
        }

        // 특정 폴더 안의 하위 폴더 목록을 불러오는 함수 (개선 1)
        // 자동 정리로 만들어진 카테고리 폴더("이미지", "동영상" 등) 표시에 사용
        public static List<DirectoryInfo> LoadFolders(string folderPath)
        {
            // 폴더가 없거나 잘못된 경로면 빈 리스트 반환
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                return new List<DirectoryInfo>();

            // 폴더 정보를 DirectoryInfo로 생성하고 하위 폴더 목록을 가져옴
            return new DirectoryInfo(folderPath)
                .GetDirectories() // 폴더 안 하위 폴더들 가져오기
                .Where(d => (d.Attributes & FileAttributes.Hidden) == 0) // 숨김 폴더는 제외
                .OrderBy(d => d.Name) // 이름 오름차순 정렬
                .ToList(); // List<DirectoryInfo>로 변환
        }

        // 파일을 기본 프로그램으로 여는 함수
        public static void OpenFile(string path)
        {
            // 파일이 존재하지 않으면 실행하지 않음
            if (!File.Exists(path))
                return;

            // 윈도우 기본 연결 프로그램으로 파일 열기
            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true // 기본 프로그램으로 실행하기 위한 설정
            });
        }

        // 파일을 삭제하는 함수
        public static void DeleteFile(string path)
        {
            // 파일이 존재하면 삭제
            if (File.Exists(path))
                File.Delete(path);
        }

        // 파일을 다른 폴더로 복사하는 함수
        public static void CopyFile(string sourcePath, string targetFolder)
        {
            // 복사될 파일의 최종 경로 생성
            string destinationPath = Path.Combine(
                targetFolder, // 사용자가 선택한 대상 폴더
                Path.GetFileName(sourcePath) // 원본 파일 이름
            );

            // 파일 복사, 같은 이름이 있으면 덮어쓰기
            File.Copy(sourcePath, destinationPath, true);
        }

        // 파일을 다른 폴더로 이동하는 함수
        public static void MoveFile(string sourcePath, string targetFolder)
        {
            // 이동될 파일의 최종 경로 생성
            string destinationPath = Path.Combine(
                targetFolder, // 사용자가 선택한 대상 폴더
                Path.GetFileName(sourcePath) // 원본 파일 이름
            );

            // 파일 이동
            File.Move(sourcePath, destinationPath);
        }

        // 파일 이름을 변경하는 함수
        public static void RenameFile(string oldPath, string newName)
        {
            // 기존 파일이 들어있는 폴더 경로 가져오기
            string folder = Path.GetDirectoryName(oldPath);

            // 기존 파일 확장자 가져오기
            string ext = Path.GetExtension(oldPath);

            // 새 파일 경로 생성
            string newPath = Path.Combine(folder, newName + ext);

            // 파일 이름 변경
            File.Move(oldPath, newPath);
        }
    }
}