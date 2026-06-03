// ==========================================
// 파일 미리보기 영역을 관리하는 Service 클래스
//
// 주요 기능:
// - 선택한 파일 정보 표시
// - 이미지 파일 미리보기 표시
// - 파일 이름 / 크기 / 날짜 출력
// ==========================================

using System.Drawing; // Image 사용
using System.IO; // FileInfo 사용
using System.Windows.Forms; // Label, PictureBox 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 파일 미리보기 영역 업데이트를 담당하는 Service 클래스
    public static class PreviewService
    {
        // 선택한 파일 정보를 오른쪽 미리보기 패널에 표시하는 함수
        public static void UpdatePreview(
            FileInfo fi, // 선택한 파일 정보
            Label lblPreviewName, // 파일 이름 표시 Label
            Label lblPreviewSize, // 파일 크기 표시 Label
            Label lblPreviewDate, // 파일 날짜 표시 Label
            Label lblPreviewPath, // 파일 경로 표시 Label
            PictureBox pictureBoxPreview) // 이미지 미리보기 PictureBox
        {
            // 파일 이름 표시
            lblPreviewName.Text = "이름: " + fi.Name;

            // 파일 크기 표시
            lblPreviewSize.Text = "크기: " + FileFormatHelper.FormatSize(fi.Length);

            // 파일 수정 날짜 표시
            lblPreviewDate.Text = "날짜: " + fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm");

            // 파일 전체 경로 표시
            lblPreviewPath.Text = "경로: " + fi.FullName;

            // 이전에 표시되던 이미지 초기화
            pictureBoxPreview.Image = null;

            // 파일 확장자를 소문자로 가져오기
            string ext = fi.Extension.ToLower();

            // 이미지 파일인 경우만 PictureBox에 미리보기 표시
            if (ext == ".jpg" ||
                ext == ".jpeg" ||
                ext == ".png" ||
                ext == ".bmp" ||
                ext == ".gif" ||
                ext == ".webp")
            {
                try
                {
                    // 이미지 파일을 PictureBox에 표시
                    pictureBoxPreview.Image = Image.FromFile(fi.FullName);
                }
                catch
                {
                    // 이미지 로드 실패 시 미리보기 비움
                    pictureBoxPreview.Image = null;
                }
            }
        }
    }
}