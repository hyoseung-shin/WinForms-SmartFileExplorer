// ==========================================
// 자동 정리 미리보기 / 실행 확인 Form 클래스
//
// 주요 기능:
// - 어떤 파일이 어느 카테고리로 이동될지 표시
// - 카테고리별 파일 개수 요약 표시
// - 사용자가 확인하면 실제 이동 수행, 취소하면 닫힘
//
// 사용 흐름:
//   1) Form1에서 자동 정리 버튼 클릭
//   2) AutoOrganizeService.BuildPlan()으로 계획 생성
//   3) 이 Form에 계획을 넘겨 ShowDialog()
//   4) 사용자가 "정리 실행" 누르면 Execute() 호출
//   5) 결과를 Form1에 반환
// ==========================================

using System; // EventArgs, Exception 사용
using System.Collections.Generic; // List 사용
using System.Drawing; // Color, Font, Point, Size 사용
using System.IO; // Path 사용
using System.Linq; // GroupBy, OrderBy 사용
using System.Windows.Forms; // Form 등 UI 컨트롤 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 자동 정리 미리보기/실행 다이얼로그
    public class AutoOrganizeForm : Form
    {
        // ---- UI 컨트롤 ----
        private Label lblTitle; // 상단 제목 라벨
        private Label lblSummary; // 카테고리별 요약 라벨
        private ListView listPreview; // 파일 이동 미리보기 리스트
        private Button btnRun; // "정리 실행" 버튼
        private Button btnCancel; // "취소" 버튼
        private Label lblHint; // 안내 문구

        // ---- 외부에서 전달받는 데이터 ----
        private List<OrganizePlanItem> _plan; // 정리 계획

        // ---- 외부에서 결과를 읽을 수 있는 속성 ----
        // 정리 실행 결과 (취소 시 null)
        public AutoOrganizeResult Result { get; private set; }

        // 생성자: 계획을 받아 UI를 구성하고 데이터를 채움
        public AutoOrganizeForm(List<OrganizePlanItem> plan)
        {
            // 외부에서 받은 계획 저장 (null이면 빈 리스트로 안전 처리)
            _plan = plan ?? new List<OrganizePlanItem>();

            // UI 초기화
            InitializeUI();

            // 데이터 채우기
            FillData();
        }

        // UI 컨트롤 생성 및 배치
        private void InitializeUI()
        {
            // 창 기본 설정
            this.Text = "자동 정리 미리보기";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(640, 520);
            this.BackColor = Color.White;

            // 제목 라벨
            lblTitle = new Label();
            lblTitle.Text = "자동 정리 미리보기";
            lblTitle.Font = new Font("맑은 고딕", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
            lblTitle.Location = new Point(20, 16);
            lblTitle.Size = new Size(600, 32);

            // 요약 라벨(카테고리별 개수)
            lblSummary = new Label();
            lblSummary.Font = new Font("맑은 고딕", 10F);
            lblSummary.ForeColor = Color.FromArgb(60, 60, 60);
            lblSummary.Location = new Point(20, 50);
            lblSummary.Size = new Size(600, 90);
            lblSummary.TextAlign = ContentAlignment.TopLeft;

            // 미리보기 리스트뷰
            listPreview = new ListView();
            listPreview.View = View.Details;
            listPreview.FullRowSelect = true;
            listPreview.GridLines = true;
            listPreview.MultiSelect = false;
            listPreview.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listPreview.Font = new Font("맑은 고딕", 9.5F);
            listPreview.Location = new Point(20, 150);
            listPreview.Size = new Size(600, 290);

            // 컬럼 추가
            listPreview.Columns.Add("파일 이름", 250);
            listPreview.Columns.Add("→ 분류", 80);
            listPreview.Columns.Add("→ 이동 후 이름", 240);

            // 안내 문구
            lblHint = new Label();
            lblHint.Text = "※ 분류 폴더가 없으면 자동으로 생성되며, 이름이 같은 파일이 있으면 \"(1)\", \"(2)\" 형태로 안전하게 저장됩니다.";
            lblHint.Font = new Font("맑은 고딕", 9F);
            lblHint.ForeColor = Color.FromArgb(110, 110, 110);
            lblHint.Location = new Point(20, 448);
            lblHint.Size = new Size(600, 30);

            // 정리 실행 버튼
            btnRun = new Button();
            btnRun.Text = "✨ 정리 실행";
            btnRun.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            btnRun.BackColor = Color.FromArgb(0, 120, 215);
            btnRun.ForeColor = Color.White;
            btnRun.FlatStyle = FlatStyle.Flat;
            btnRun.FlatAppearance.BorderSize = 0;
            btnRun.Location = new Point(400, 478);
            btnRun.Size = new Size(110, 32);
            btnRun.Cursor = Cursors.Hand;
            btnRun.Click += btnRun_Click;

            // 취소 버튼
            btnCancel = new Button();
            btnCancel.Text = "취소";
            btnCancel.Font = new Font("맑은 고딕", 10F);
            btnCancel.BackColor = Color.FromArgb(240, 240, 240);
            btnCancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Location = new Point(520, 478);
            btnCancel.Size = new Size(100, 32);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Click += btnCancel_Click;

            // 컨트롤들을 Form에 추가
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSummary);
            this.Controls.Add(listPreview);
            this.Controls.Add(lblHint);
            this.Controls.Add(btnRun);
            this.Controls.Add(btnCancel);

            // ESC 키 처리
            this.CancelButton = btnCancel;
        }

        // 미리보기 데이터 채우기
        private void FillData()
        {
            // 요약 텍스트 채우기 (카테고리별 개수)
            lblSummary.Text = "다음과 같이 분류 후 이동됩니다.\n\n"
                + AutoOrganizeService.SummarizePlan(_plan);

            // 리스트뷰 초기화
            listPreview.BeginUpdate();
            listPreview.Items.Clear();

            // 계획이 없으면 안내만 출력하고 실행 버튼 비활성화
            if (_plan == null || _plan.Count == 0)
            {
                btnRun.Enabled = false;
                listPreview.EndUpdate();
                return;
            }

            // 카테고리별로 묶어서 출력(보기 좋게)
            var grouped = _plan
                .GroupBy(p => p.CategoryName)
                .OrderBy(g => g.Key);

            // 그룹 표시 활성화
            listPreview.ShowGroups = true;
            listPreview.Groups.Clear();

            // 카테고리별 그룹 생성 후 항목 추가
            foreach (var g in grouped)
            {
                // 카테고리 그룹 생성 ("이미지 (5개)" 형태)
                ListViewGroup lvGroup = new ListViewGroup(g.Key + " (" + g.Count() + "개)");
                listPreview.Groups.Add(lvGroup);

                // 해당 카테고리의 항목 추가
                foreach (OrganizePlanItem item in g.OrderBy(x => x.Source.Name))
                {
                    // 항목 1줄 = (원본 이름, 카테고리, 이동 후 파일명)
                    ListViewItem lvi = new ListViewItem(item.Source.Name);
                    lvi.SubItems.Add(item.CategoryName);

                    // 이동 후 파일명만 표시(전체 경로는 너무 길어서 생략)
                    string movedName = Path.GetFileName(item.TargetFullPath);

                    // 이름이 바뀐 경우 표시(* 표시)
                    if (item.IsRenamed)
                        movedName = "* " + movedName;

                    lvi.SubItems.Add(movedName);
                    lvi.Group = lvGroup;

                    listPreview.Items.Add(lvi);
                }
            }

            // 리스트뷰 업데이트 완료
            listPreview.EndUpdate();
        }

        // "정리 실행" 버튼 클릭
        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                // 사용자에게 한번 더 확인
                DialogResult dr = MessageBox.Show(
                    _plan.Count + "개의 파일을 분류 폴더로 이동합니다.\n계속하시겠습니까?",
                    "자동 정리 실행",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                // 아니오면 그대로 머무름
                if (dr != DialogResult.Yes)
                    return;

                // 마우스 커서를 대기 상태로
                this.Cursor = Cursors.WaitCursor;

                // 자동 정리 실행
                Result = AutoOrganizeService.Execute(_plan);

                // 커서 원복
                this.Cursor = Cursors.Default;

                // 결과 메시지 만들기
                string msg = "정리가 완료되었습니다.\n\n"
                    + " • 성공: " + Result.SuccessCount + "개\n"
                    + " • 실패: " + Result.FailedCount + "개\n"
                    + " • 건너뜀: " + Result.SkippedCount + "개\n"
                    + " • 생성된 폴더 수: " + Result.CreatedFolders.Count + "개";

                // 실패가 있으면 첫 오류 메시지를 보여줌
                if (Result.FailedCount > 0 && Result.Errors.Count > 0)
                    msg += "\n\n[오류 예시]\n" + Result.Errors[0];

                // 결과 메시지 표시
                MessageBox.Show(msg, "자동 정리 결과", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 정상 종료 상태로 닫기
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // 예외 발생 시 커서 원복 및 오류 메시지 표시
                this.Cursor = Cursors.Default;
                MessageBox.Show("자동 정리 실행 중 오류가 발생했습니다.\n" + ex.Message,
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // "취소" 버튼 클릭
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // 결과는 null인 상태로 종료(취소)
            Result = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
