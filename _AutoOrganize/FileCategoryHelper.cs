// ==========================================
// 파일 확장자를 기반으로
// 파일 종류(이미지/문서/동영상 등)를
// 분류하는 Helper 클래스
//
// 주요 기능:
// - 확장자별 카테고리 반환
// - 전체 카테고리 이름 목록 반환
//   (자동 정리 시 카테고리 폴더 생성에 사용)
// ==========================================

using System; // StringComparer 사용
using System.Collections.Generic; // Dictionary, List 사용
using System.IO; // FileInfo 사용
using System.Linq; // Distinct, ToList 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 파일 확장자를 기준으로 파일 종류를 분류하는 Helper 클래스
    public static class FileCategoryHelper
    {
        // "기타" 카테고리 이름을 상수로 둠
        // (자동 정리에서 분류되지 않은 파일이 들어갈 폴더 이름)
        public const string CategoryEtc = "기타";

        // 확장자별 파일 종류를 저장하는 Dictionary
        private static readonly Dictionary<string, string> ExtMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) // 대소문자 구분 없이 확장자 비교
        {
            {".jpg","이미지"}, {".jpeg","이미지"}, {".png","이미지"}, {".gif","이미지"}, {".bmp","이미지"}, {".webp","이미지"},
            {".mp4","동영상"}, {".mkv","동영상"}, {".avi","동영상"}, {".mov","동영상"}, {".wmv","동영상"},
            {".pdf","문서"}, {".doc","문서"}, {".docx","문서"}, {".xls","문서"}, {".xlsx","문서"},
            {".pptx","문서"}, {".txt","문서"}, {".hwp","문서"},
            {".zip","압축파일"}, {".rar","압축파일"}, {".7z","압축파일"}, {".tar","압축파일"}, {".gz","압축파일"},
            {".exe","실행파일"}, {".msi","실행파일"}, {".bat","실행파일"}
        };

        // FileInfo를 받아 해당 파일의 종류를 반환하는 함수
        public static string GetCategory(FileInfo fi)
        {
            string cat; // Dictionary에서 찾은 파일 종류를 담을 변수

            // 확장자가 Dictionary에 있으면 해당 종류 반환, 없으면 "기타" 반환
            return ExtMap.TryGetValue(fi.Extension, out cat) ? cat : CategoryEtc;
        }

        // 등록된 모든 카테고리 이름 목록을 반환하는 함수
        // 자동 정리에서 "이미 카테고리 폴더 안에 들어있는 파일은 정리 대상에서 제외"하는 데 사용됨
        public static List<string> GetAllCategories()
        {
            // ExtMap에 등록된 카테고리 이름을 중복 없이 모으고 "기타"를 추가
            List<string> categories = ExtMap.Values
                .Distinct() // 중복 제거
                .ToList();

            // "기타" 카테고리가 빠져있으면 추가 (분류되지 않은 파일이 가는 폴더)
            if (!categories.Contains(CategoryEtc))
                categories.Add(CategoryEtc);

            // 결과 반환
            return categories;
        }
    }
}
