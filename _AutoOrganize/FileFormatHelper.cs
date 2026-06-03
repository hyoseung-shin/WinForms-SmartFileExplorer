// ==========================================
// 파일 표시 형식을 담당하는 Helper 클래스
//
// 주요 기능:
// - 파일 크기 포맷 변환
//   (B / KB / MB / GB)
//
// - 파일 날짜 그룹 분류
//   (오늘 / 어제 / 이번 주 등)
// ==========================================

using System; // DateTime 사용
using System.Windows.Forms; // ListViewGroup 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 파일 크기, 날짜 그룹 같은 표시 형식을 담당하는 Helper 클래스
    public static class FileFormatHelper
    {
        // byte 단위 파일 크기를 사람이 보기 쉬운 단위로 변환하는 함수
        public static string FormatSize(long bytes)
        {
            // 1024보다 작으면 B 단위로 표시
            if (bytes < 1024)
                return bytes + " B";

            // 1MB보다 작으면 KB 단위로 표시
            if (bytes < 1024 * 1024)
                return (bytes / 1024.0).ToString("F1") + " KB";

            // 1GB보다 작으면 MB 단위로 표시
            if (bytes < 1024L * 1024 * 1024)
                return (bytes / (1024.0 * 1024)).ToString("F1") + " MB";

            // 그 이상은 GB 단위로 표시
            return (bytes / (1024.0 * 1024 * 1024)).ToString("F2") + " GB";
        }

        // 파일 날짜를 기준으로 오늘/어제/이번 주/지난 주/지난 달/오래 전 그룹 중 하나를 반환하는 함수
        public static ListViewGroup GetDateGroup(
            DateTime fileDate, // 파일 수정 날짜
            ListViewGroup today, // 오늘 그룹
            ListViewGroup yesterday, // 어제 그룹
            ListViewGroup thisWeek, // 이번 주 그룹
            ListViewGroup lastWeek, // 지난 주 그룹
            ListViewGroup lastMonth, // 지난 달 그룹
            ListViewGroup older) // 오래 전 그룹
        {
            DateTime now = DateTime.Now.Date; // 오늘 날짜
            DateTime date = fileDate.Date; // 파일 수정 날짜에서 날짜 부분만 추출

            int diffDays = (now - date).Days; // 오늘과 파일 날짜의 차이 계산

            if (diffDays == 0) return today; // 오늘 수정된 파일
            if (diffDays == 1) return yesterday; // 어제 수정된 파일
            if (diffDays <= 7) return thisWeek; // 7일 이내 수정된 파일
            if (diffDays <= 14) return lastWeek; // 14일 이내 수정된 파일
            if (diffDays <= 31) return lastMonth; // 31일 이내 수정된 파일

            return older; // 그보다 오래된 파일
        }
    }
}