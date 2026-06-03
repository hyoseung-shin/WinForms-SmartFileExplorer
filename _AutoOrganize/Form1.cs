// ==========================================
// 메인 UI 화면(Form)
// 사용자 입력 및 버튼 이벤트 처리 담당
//
// 주요 기능:
// - 파일 목록 표시
// - 검색 / 필터 / 정렬
// - 파일 선택 처리
// - 버튼 클릭 이벤트 처리
// - Service 클래스 호출
// ==========================================

using System; // 기본 기능, EventArgs, Exception 사용
using System.Collections.Generic; // List 사용
using System.IO; // FileInfo, File, Path 사용
using System.Linq; // Where, OrderBy, Sum, ToList 사용
using System.Windows.Forms; // WinForms UI 기능 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 메인 화면 Form 클래스
    public partial class Form1 : Form
    {
        // 현재 열려 있는 폴더 경로 저장
        private string _currentFolder = string.Empty;

        // 현재 폴더 안의 전체 파일 목록 저장
        private List<FileInfo> _allFiles = new List<FileInfo>();

        // 현재 폴더 안의 하위 폴더 목록 저장 (개선 1: 폴더 표시)
        private List<DirectoryInfo> _allFolders = new List<DirectoryInfo>();

        // 정렬 방향 저장, true면 오름차순, false면 내림차순
        private bool _sortAscending = true;

        // ────────────────────────────────────────────────
        // 개선 2: 되돌리기 기능
        // 직전 자동 정리 결과(이동 내역)를 보관
        // ────────────────────────────────────────────────
        private AutoOrganizeResult _lastOrganizeResult = null;

        // ────────────────────────────────────────────────
        // 개선 3: 주기적 알림 (정리 대상 감지)
        // ────────────────────────────────────────────────
        // 30초마다 정리 대상이 있는지 확인하는 Timer (배너 알림만 함, 실제 이동 안 함)
        private System.Windows.Forms.Timer _scanTimer;

        // 알림 배너에서 사용자가 한 번 "숨기기"한 항목 수
        // 같은 개수에 대해서는 재알림하지 않기 위함
        private int _lastNotifiedCount = -1;

        // 알림 배너가 현재 표시 중인지 여부
        private bool _isNotificationVisible = false;

        // ────────────────────────────────────────────────
        // 피드백 ②: 5초마다 폴더 변경을 감지하는 Timer
        //   - 폴더 내용이 외부에서 바뀌면(새 파일 추가/삭제) ListView를 자동 갱신
        //   - 실제 파일 이동은 절대 하지 않음 (목록 표시만)
        // ────────────────────────────────────────────────
        private System.Windows.Forms.Timer _refreshTimer;

        // 마지막으로 확인한 폴더의 LastWriteTime
        // 이 값과 비교해 변화가 있을 때만 LoadFiles() 호출 (불필요한 갱신 방지)
        private DateTime _lastFolderWriteTime = DateTime.MinValue;

        // ────────────────────────────────────────────────
        // 피드백 ①: 폴더 탐색 히스토리 (브라우저식 뒤로/앞으로)
        // ────────────────────────────────────────────────
        // 뒤로 가기 스택: 이전에 방문한 폴더 경로
        private Stack<string> _backStack = new Stack<string>();

        // 앞으로 가기 스택: 뒤로 가기로 이탈한 폴더 경로
        private Stack<string> _forwardStack = new Stack<string>();

        // Form1 생성자, 프로그램 창이 처음 열릴 때 실행됨
        public Form1()
        {
            // 디자이너에서 만든 UI 요소들을 초기화
            InitializeComponent();

            // 정렬 콤보박스의 기본 선택값을 3번으로 설정
            // 3번은 수정 날짜 정렬이라고 가정
            cmbSort.SelectedIndex = 3;

            // 기본 정렬 방향을 내림차순으로 설정
            // 최신 파일이 위로 오게 하기 위함
            _sortAscending = false;

            // 다운로드 폴더 경로를 가져와 현재 폴더로 설정
            string downloadsPath = FileService.GetDownloadsPath();

            // 피드백 ①: 초기 폴더는 NavigateTo로 진입(히스토리에는 추가하지 않음)
            NavigateTo(downloadsPath, addToHistory: false);

            // ────────────────────────────────────────
            // 피드백 ②: 5초마다 폴더 변화를 감지해 ListView 자동 갱신
            //   - 새 파일이 추가되거나 외부에서 변경이 발생하면 자동으로 반영
            //   - 절대 파일을 이동시키지 않음 (목록 표시만)
            // ────────────────────────────────────────
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000; // 5초 간격
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            // ────────────────────────────────────────
            // 개선 3: 30초마다 정리 대상이 있는지 검사
            //   - 정리 대상이 있으면 알림 배너만 띄움
            //   - 절대 파일을 이동시키지 않음 (알림만)
            // ────────────────────────────────────────
            _scanTimer = new System.Windows.Forms.Timer();
            _scanTimer.Interval = 30000; // 30초 간격
            _scanTimer.Tick += ScanTimer_Tick;
            _scanTimer.Start();

            // Form이 닫힐 때 Timer 정리
            this.FormClosed += (s, e) =>
            {
                if (_refreshTimer != null)
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Dispose();
                }
                if (_scanTimer != null)
                {
                    _scanTimer.Stop();
                    _scanTimer.Dispose();
                }
            };
        }

        // ════════════════════════════════════════════════════
        //  피드백 ①: 폴더 탐색 시스템 (브라우저식 뒤로/앞으로/위로)
        // ════════════════════════════════════════════════════

        // 폴더로 이동하는 중앙 함수
        //   - 모든 폴더 이동은 반드시 이 함수를 거치도록 통일
        //   - addToHistory=true: 현재 폴더를 _backStack에 추가 (일반 폴더 진입)
        //   - addToHistory=false: 히스토리 변경 없음 (초기 로딩, 뒤로/앞으로 가기)
        private void NavigateTo(string targetFolder, bool addToHistory = true)
        {
            // 빈 경로 또는 존재하지 않는 폴더는 무시
            if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder))
            {
                SetStatus("폴더를 열 수 없습니다: " + targetFolder);
                return;
            }

            // 같은 폴더로의 이동은 무시 (불필요한 히스토리 누적 방지)
            if (string.Equals(targetFolder, _currentFolder, StringComparison.OrdinalIgnoreCase))
                return;

            // 새로운 폴더 진입 시 현재 폴더를 뒤로 가기 스택에 저장
            if (addToHistory && !string.IsNullOrEmpty(_currentFolder))
            {
                _backStack.Push(_currentFolder);

                // 새 경로로 진입했으므로 앞으로 가기 히스토리는 초기화
                // (브라우저와 동일한 동작)
                _forwardStack.Clear();
            }

            // 현재 폴더 변경
            _currentFolder = targetFolder;

            // 폴더가 바뀌면 알림 추적 상태도 리셋
            _lastNotifiedCount = -1;
            HideNotification();

            // 새 폴더의 파일/폴더 목록 로드
            LoadFiles();

            // 주소표시줄/네비게이션 버튼 UI 갱신
            UpdateNavigationUI();
        }

        // 뒤로 가기 버튼 클릭
        private void btnBack_Click(object sender, EventArgs e)
        {
            // 뒤로 갈 곳이 없으면 무시
            if (_backStack.Count == 0)
                return;

            // 현재 폴더를 앞으로 가기 스택에 저장
            _forwardStack.Push(_currentFolder);

            // 뒤로 가기 스택에서 가장 최근 폴더 꺼내기
            string previousFolder = _backStack.Pop();

            // 히스토리에 추가하지 않고 이동(이미 스택 조작이 끝났으므로)
            NavigateTo(previousFolder, addToHistory: false);
        }

        // 앞으로 가기 버튼 클릭
        private void btnForward_Click(object sender, EventArgs e)
        {
            // 앞으로 갈 곳이 없으면 무시
            if (_forwardStack.Count == 0)
                return;

            // 현재 폴더를 뒤로 가기 스택에 저장
            _backStack.Push(_currentFolder);

            // 앞으로 가기 스택에서 가장 최근 폴더 꺼내기
            string nextFolder = _forwardStack.Pop();

            // 히스토리에 추가하지 않고 이동
            NavigateTo(nextFolder, addToHistory: false);
        }

        // 상위 폴더로 가기 버튼 클릭
        private void btnUp_Click(object sender, EventArgs e)
        {
            // 부모 폴더 경로 가져오기
            string parent = Path.GetDirectoryName(_currentFolder);

            // 최상위 폴더이거나 부모가 없으면 무시
            if (string.IsNullOrEmpty(parent) || !Directory.Exists(parent))
            {
                SetStatus("상위 폴더가 없습니다.");
                return;
            }

            // 일반 폴더 진입과 동일하게 히스토리에 현재 폴더 추가
            NavigateTo(parent, addToHistory: true);
        }

        // 주소표시줄/네비게이션 버튼의 UI 상태를 현재 히스토리에 맞춰 갱신
        private void UpdateNavigationUI()
        {
            // 주소 표시줄: 폴더 경로를 사람이 읽기 좋게 표시 ("🖥  >  Downloads  >  이미지")
            lblAddress.Text = "  🖥  >  " + GetDisplayPath(_currentFolder);

            // 뒤로 갈 폴더가 있으면 진하게, 없으면 회색
            btnBack.ForeColor = _backStack.Count > 0
                ? System.Drawing.Color.FromArgb(30, 30, 30)
                : System.Drawing.Color.LightGray;
            btnBack.Enabled = _backStack.Count > 0;

            // 앞으로 갈 폴더가 있으면 진하게, 없으면 회색
            btnForward.ForeColor = _forwardStack.Count > 0
                ? System.Drawing.Color.FromArgb(30, 30, 30)
                : System.Drawing.Color.LightGray;
            btnForward.Enabled = _forwardStack.Count > 0;

            // 상위 폴더 존재 여부에 따라 ↑ 버튼 활성/비활성
            string parent = Path.GetDirectoryName(_currentFolder);
            bool canGoUp = !string.IsNullOrEmpty(parent) && Directory.Exists(parent);
            btnUp.ForeColor = canGoUp
                ? System.Drawing.Color.FromArgb(30, 30, 30)
                : System.Drawing.Color.LightGray;
            btnUp.Enabled = canGoUp;
        }

        // 폴더 경로를 보기 좋은 형태로 변환
        //   - 다운로드 폴더와 그 하위는 "Downloads > 이미지" 형태로 단순화
        //   - 그 외에는 전체 경로 표시
        private string GetDisplayPath(string fullPath)
        {
            // 입력이 비어 있으면 빈 문자열 반환
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            // 다운로드 폴더 경로 가져오기
            string downloadsPath = FileService.GetDownloadsPath();

            // 현재 폴더가 다운로드 폴더와 같으면 "Downloads"로 표시
            if (string.Equals(fullPath, downloadsPath, StringComparison.OrdinalIgnoreCase))
                return "Downloads";

            // 현재 폴더가 다운로드 폴더의 하위라면 상대 경로로 표시
            if (fullPath.StartsWith(downloadsPath, StringComparison.OrdinalIgnoreCase))
            {
                string relative = fullPath.Substring(downloadsPath.Length).TrimStart('\\', '/');
                return "Downloads  >  " + relative.Replace("\\", "  >  ");
            }

            // 그 외에는 전체 경로 그대로 표시
            return fullPath;
        }

        // 현재 폴더의 파일 목록을 다시 불러오는 함수
        private void LoadFiles()
        {
            try
            {
                // FileService를 통해 실제 파일 목록을 읽어옴
                _allFiles = FileService.LoadFiles(_currentFolder);

                // 개선 1: 하위 폴더 목록도 함께 로드 (정리 후 카테고리 폴더 표시용)
                _allFolders = FileService.LoadFolders(_currentFolder);

                // 피드백 ②: 현재 폴더의 마지막 쓰기 시간 기록
                // RefreshTimer가 이 값과 비교해 변화 감지에 사용
                if (Directory.Exists(_currentFolder))
                    _lastFolderWriteTime = new DirectoryInfo(_currentFolder).LastWriteTime;

                // 검색, 필터, 정렬을 적용한 뒤 화면에 표시
                ApplyFilterAndSort();

                // 파일 개수, 총 크기 같은 상태 정보를 업데이트
                UpdateStatus();
            }
            catch (Exception ex)
            {
                // 오류가 발생하면 상태 라벨에 오류 메시지 표시
                SetStatus("오류: " + ex.Message);
            }
        }

        // 검색어, 필터, 정렬 기준을 적용하는 함수
        private void ApplyFilterAndSort()
        {
            // 검색창의 텍스트를 가져오고 앞뒤 공백 제거 후 소문자로 변환
            string search = txtSearch.Text.Trim().ToLower();

            // 필터 콤보박스에서 선택된 값이 없으면 "전체"로 처리
            string filterCat = cmbFilter.SelectedItem == null
                ? "전체"
                : cmbFilter.SelectedItem.ToString();

            // 전체 파일 목록을 필터링 대상 목록으로 설정
            IEnumerable<FileInfo> filtered = _allFiles;

            // 검색어가 비어 있지 않으면 파일 이름에 검색어가 포함된 파일만 남김
            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(f => f.Name.ToLower().Contains(search));

            // 필터가 "전체"가 아니면 해당 종류의 파일만 남김
            if (filterCat != "전체")
                filtered = filtered.Where(f => FileCategoryHelper.GetCategory(f) == filterCat);

            // 정렬 기준이 0번이면 파일 이름 기준 정렬
            if (cmbSort.SelectedIndex == 0)
            {
                // 정렬 방향에 따라 오름차순 또는 내림차순 정렬
                filtered = _sortAscending
                    ? filtered.OrderBy(f => f.Name)
                    : filtered.OrderByDescending(f => f.Name);
            }
            // 정렬 기준이 1번이면 파일 크기 기준 정렬
            else if (cmbSort.SelectedIndex == 1)
            {
                // 정렬 방향에 따라 오름차순 또는 내림차순 정렬
                filtered = _sortAscending
                    ? filtered.OrderBy(f => f.Length)
                    : filtered.OrderByDescending(f => f.Length);
            }
            // 정렬 기준이 2번이면 파일 종류 기준 정렬
            else if (cmbSort.SelectedIndex == 2)
            {
                // 정렬 방향에 따라 오름차순 또는 내림차순 정렬
                filtered = _sortAscending
                    ? filtered.OrderBy(f => FileCategoryHelper.GetCategory(f))
                    : filtered.OrderByDescending(f => FileCategoryHelper.GetCategory(f));
            }
            // 정렬 기준이 3번이면 수정 날짜 기준 정렬
            else if (cmbSort.SelectedIndex == 3)
            {
                // 정렬 방향에 따라 오름차순 또는 내림차순 정렬
                filtered = _sortAscending
                    ? filtered.OrderBy(f => f.LastWriteTime)
                    : filtered.OrderByDescending(f => f.LastWriteTime);
            }

            // 최종 필터링/정렬된 결과를 ListView에 표시
            PopulateList(filtered.ToList());
        }

        // 파일 목록을 ListView에 출력하는 함수
        private void PopulateList(List<FileInfo> files)
        {
            // ListView 업데이트 시작, 깜빡임 방지 목적
            listViewFiles.BeginUpdate();

            // 기존 파일 항목 삭제
            listViewFiles.Items.Clear();

            // 기존 날짜 그룹 삭제
            listViewFiles.Groups.Clear();

            // ────────────────────────────────────────
            // 개선 1: 폴더 그룹 (최상단)
            // 자동 정리로 만들어진 카테고리 폴더가 여기에 표시됨
            // ────────────────────────────────────────
            ListViewGroup groupFolders = new ListViewGroup("폴더");
            listViewFiles.Groups.Add(groupFolders);

            // 오늘 그룹 생성
            ListViewGroup groupToday = new ListViewGroup("오늘");

            // 어제 그룹 생성
            ListViewGroup groupYesterday = new ListViewGroup("어제");

            // 이번 주 그룹 생성
            ListViewGroup groupThisWeek = new ListViewGroup("이번 주");

            // 지난 주 그룹 생성
            ListViewGroup groupLastWeek = new ListViewGroup("지난 주");

            // 지난 달 그룹 생성
            ListViewGroup groupLastMonth = new ListViewGroup("지난 달");

            // 오래 전 그룹 생성
            ListViewGroup groupOlder = new ListViewGroup("오래 전");

            // ListView에 오늘 그룹 추가
            listViewFiles.Groups.Add(groupToday);

            // ListView에 어제 그룹 추가
            listViewFiles.Groups.Add(groupYesterday);

            // ListView에 이번 주 그룹 추가
            listViewFiles.Groups.Add(groupThisWeek);

            // ListView에 지난 주 그룹 추가
            listViewFiles.Groups.Add(groupLastWeek);

            // ListView에 지난 달 그룹 추가
            listViewFiles.Groups.Add(groupLastMonth);

            // ListView에 오래 전 그룹 추가
            listViewFiles.Groups.Add(groupOlder);

            // ────────────────────────────────────────
            // 개선 1: 하위 폴더들을 최상단에 추가
            //   - 검색어가 있으면 폴더 이름으로 필터링
            //   - 필터가 "전체"가 아니면 폴더는 숨김(파일 카테고리 필터이므로)
            // ────────────────────────────────────────
            string searchLower = txtSearch.Text.Trim().ToLower();
            string currentFilter = cmbFilter.SelectedItem == null
                ? "전체"
                : cmbFilter.SelectedItem.ToString();

            int visibleFolderCount = 0;
            if (currentFilter == "전체")
            {
                foreach (DirectoryInfo di in _allFolders)
                {
                    // 검색어가 있는데 폴더명에 포함되지 않으면 스킵
                    if (!string.IsNullOrEmpty(searchLower)
                        && !di.Name.ToLower().Contains(searchLower))
                        continue;

                    // 폴더 1행 만들기
                    // 첫 번째 열에 폴더 아이콘과 이름
                    ListViewItem folderItem = new ListViewItem("📁 " + di.Name);

                    // 두 번째 열: 크기 대신 "(폴더)" 표시
                    folderItem.SubItems.Add("(폴더)");

                    // 세 번째 열: 종류
                    folderItem.SubItems.Add("폴더");

                    // 네 번째 열: 수정 날짜
                    folderItem.SubItems.Add(di.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                    // 폴더임을 구분하기 위해 Tag 앞에 "DIR:" 접두어를 붙여 저장
                    // (DoubleClick에서 파일/폴더 구분에 사용)
                    folderItem.Tag = "DIR:" + di.FullName;

                    // 폴더 행은 진한 색으로 강조해 시각적 구분
                    folderItem.ForeColor = System.Drawing.Color.FromArgb(0, 90, 158);
                    folderItem.Font = new System.Drawing.Font(listViewFiles.Font, System.Drawing.FontStyle.Bold);

                    // "폴더" 그룹에 배치
                    folderItem.Group = groupFolders;

                    // ListView에 추가
                    listViewFiles.Items.Add(folderItem);
                    visibleFolderCount++;
                }
            }

            // 폴더가 한 개도 없으면 폴더 그룹을 보이지 않게 처리
            if (visibleFolderCount == 0)
                listViewFiles.Groups.Remove(groupFolders);

            // 표시할 파일 목록을 하나씩 반복
            foreach (FileInfo fi in files)
            {
                // ListView에 들어갈 한 줄짜리 항목 생성
                // 첫 번째 열에는 파일 이름이 들어감
                ListViewItem item = new ListViewItem(fi.Name);

                // 두 번째 열에 파일 크기 추가
                item.SubItems.Add(FileFormatHelper.FormatSize(fi.Length));

                // 세 번째 열에 파일 종류 추가
                item.SubItems.Add(FileCategoryHelper.GetCategory(fi));

                // 네 번째 열에 수정 날짜 추가
                item.SubItems.Add(fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                // 항목의 숨겨진 값으로 실제 파일 경로 저장
                item.Tag = fi.FullName;

                // 파일 수정 날짜에 따라 날짜 그룹 지정
                item.Group = FileFormatHelper.GetDateGroup(
                    fi.LastWriteTime,
                    groupToday,
                    groupYesterday,
                    groupThisWeek,
                    groupLastWeek,
                    groupLastMonth,
                    groupOlder
                );

                // 완성된 항목을 ListView에 추가
                listViewFiles.Items.Add(item);
            }

            // ListView에서 그룹 표시 활성화
            listViewFiles.ShowGroups = true;

            // ListView 업데이트 종료
            listViewFiles.EndUpdate();

            // 현재 화면에 표시된 파일/폴더 개수 표시 (개선 1)
            if (visibleFolderCount > 0)
                lblFileCount.Text = "폴더 " + visibleFolderCount + "개, 파일 " + files.Count + "개";
            else
                lblFileCount.Text = files.Count + "개 파일";
        }

        // 상태 표시줄 정보를 업데이트하는 함수
        private void UpdateStatus(string msg = "준비")
        {
            // 상태 메시지 라벨 업데이트
            lblStatus.Text = msg;

            // 전체 파일 크기의 합 계산
            long total = _allFiles.Sum(f => f.Length);

            // 전체 파일 크기를 보기 좋은 형식으로 표시
            lblTotalSize.Text = "총 크기: " + FileFormatHelper.FormatSize(total);
        }

        // 상태 메시지만 바꾸는 함수
        private void SetStatus(string msg)
        {
            // 상태 라벨 텍스트 변경
            lblStatus.Text = msg;
        }

        // 새로고침 버튼 클릭 이벤트
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // 파일 목록 다시 불러오기
            LoadFiles();

            // 상태 메시지 변경
            SetStatus("새로고침 완료");
        }

        // 검색창 텍스트가 바뀔 때 실행되는 이벤트
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // 검색어 변경에 맞춰 목록 다시 필터링
            ApplyFilterAndSort();
        }

        // 필터 콤보박스 선택값이 바뀔 때 실행되는 이벤트
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 필터 변경에 맞춰 목록 다시 필터링
            ApplyFilterAndSort();
        }

        // 정렬 콤보박스 선택값이 바뀔 때 실행되는 이벤트
        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 정렬 기준 변경에 맞춰 목록 다시 정렬
            ApplyFilterAndSort();
        }

        // 오름차순/내림차순 버튼 클릭 이벤트
        private void btnSortOrder_Click(object sender, EventArgs e)
        {
            // 현재 정렬 방향을 반대로 변경
            _sortAscending = !_sortAscending;

            // 버튼 텍스트를 현재 정렬 방향에 맞게 변경
            btnSortOrder.Text = _sortAscending
                ? "▲ 오름"
                : "▼ 내림";

            // 바뀐 정렬 방향을 적용하여 목록 다시 표시
            ApplyFilterAndSort();
        }

        // ListView에서 파일 선택이 바뀔 때 실행되는 이벤트
        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 선택된 파일이 없으면 함수 종료
            if (listViewFiles.SelectedItems.Count == 0)
                return;

            // 선택된 항목의 Tag에서 실제 파일 경로 가져오기
            string path = listViewFiles.SelectedItems[0].Tag.ToString();

            // 파일이 존재하지 않으면 함수 종료
            if (!File.Exists(path))
                return;

            // 선택한 파일 정보를 미리보기 영역에 표시
            PreviewService.UpdatePreview(
                new FileInfo(path),
                lblPreviewName,
                lblPreviewSize,
                lblPreviewDate,
                lblPreviewPath,
                pictureBoxPreview
            );
        }

        // ListView 파일을 더블클릭했을 때 실행되는 이벤트
        private void listViewFiles_DoubleClick(object sender, EventArgs e)
        {
            // 선택된 항목이 없으면 종료
            if (listViewFiles.SelectedItems.Count == 0)
                return;

            // 첫 번째로 선택된 항목 가져오기
            ListViewItem selected = listViewFiles.SelectedItems[0];
            string tag = selected.Tag != null ? selected.Tag.ToString() : "";

            // 개선 1: 폴더면 NavigateTo로 진입(히스토리 자동 누적)
            if (tag.StartsWith("DIR:"))
            {
                // "DIR:" 접두어를 떼고 실제 폴더 경로 추출
                string folderPath = tag.Substring(4);

                // NavigateTo 사용 → 현재 폴더가 _backStack에 자동 저장됨 (피드백 ①)
                NavigateTo(folderPath, addToHistory: true);
                return;
            }

            // 폴더가 아니면 기존 동작 — 선택된 파일 열기
            OpenSelected();
        }

        // 열기 버튼 클릭 이벤트
        private void btnOpen_Click(object sender, EventArgs e)
        {
            // 선택된 파일 열기
            OpenSelected();
        }

        // 선택된 파일들을 여는 함수
        private void OpenSelected()
        {
            // 선택된 파일 항목들을 하나씩 반복
            foreach (ListViewItem item in listViewFiles.SelectedItems)
            {
                // 항목의 Tag에서 실제 파일 경로 가져오기
                string path = item.Tag.ToString();

                // 폴더 항목은 이 함수에서 처리하지 않음 (DoubleClick에서 별도 처리)
                if (path.StartsWith("DIR:"))
                    continue;

                try
                {
                    // FileService를 통해 파일 열기
                    FileService.OpenFile(path);
                }
                catch (Exception ex)
                {
                    // 파일 열기에 실패하면 메시지 표시
                    MessageBox.Show("열기 실패: " + ex.Message);
                }
            }
        }

        // 삭제 버튼 클릭 이벤트
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 선택된 파일이 없으면 함수 종료
            if (listViewFiles.SelectedItems.Count == 0)
                return;

            // 삭제 확인 메시지 표시
            DialogResult result = MessageBox.Show(
                "선택한 파일을 삭제하시겠습니까?",
                "삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // 사용자가 Yes를 누르지 않으면 삭제 취소
            if (result != DialogResult.Yes)
                return;

            // 선택된 파일들을 하나씩 반복
            foreach (ListViewItem item in listViewFiles.SelectedItems)
            {
                // 항목의 Tag에서 실제 파일 경로 가져오기
                string path = item.Tag.ToString();

                try
                {
                    // FileService를 통해 파일 삭제
                    FileService.DeleteFile(path);
                }
                catch (Exception ex)
                {
                    // 삭제 실패 시 메시지 표시
                    MessageBox.Show("삭제 실패: " + ex.Message);
                }
            }

            // 삭제 후 파일 목록 다시 불러오기
            LoadFiles();

            // 상태 메시지 변경
            SetStatus("삭제 완료");
        }

        // 이름 변경 버튼 클릭 이벤트
        private void btnRename_Click(object sender, EventArgs e)
        {
            // 이름 변경은 하나의 파일만 선택해야 하므로, 선택 개수가 1개가 아니면 안내
            if (listViewFiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("파일을 하나만 선택하세요.");
                return;
            }

            // 선택된 파일의 실제 경로 가져오기
            string oldPath = listViewFiles.SelectedItems[0].Tag.ToString();

            // 확장자를 제외한 기존 파일 이름 가져오기
            string oldName = Path.GetFileNameWithoutExtension(oldPath);

            // 새 파일 이름을 입력받는 창 표시
            string newName = InputDialog.Show(
                "이름 변경",
                "새 파일 이름 입력",
                oldName
            );

            // 입력값이 비어 있으면 이름 변경 취소
            if (string.IsNullOrWhiteSpace(newName))
                return;

            try
            {
                // FileService를 통해 파일 이름 변경
                FileService.RenameFile(oldPath, newName);

                // 변경 후 파일 목록 다시 불러오기
                LoadFiles();

                // 상태 메시지 변경
                SetStatus("이름 변경 완료");
            }
            catch (Exception ex)
            {
                // 이름 변경 실패 시 메시지 표시
                MessageBox.Show("이름 변경 실패: " + ex.Message);
            }
        }

        // 복사 버튼 클릭 이벤트
        private void btnCopy_Click(object sender, EventArgs e)
        {
            // 선택된 파일이 없으면 함수 종료
            if (listViewFiles.SelectedItems.Count == 0)
                return;

            // 복사할 대상 폴더를 선택하는 창 생성
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                // 폴더 선택 창에 표시할 설명 설정
                dlg.Description = "복사할 폴더 선택";

                // 사용자가 폴더를 선택하지 않으면 함수 종료
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                // 선택된 파일들을 하나씩 반복
                foreach (ListViewItem item in listViewFiles.SelectedItems)
                {
                    // 항목의 Tag에서 실제 파일 경로 가져오기
                    string src = item.Tag.ToString();

                    try
                    {
                        // FileService를 통해 파일 복사
                        FileService.CopyFile(src, dlg.SelectedPath);
                    }
                    catch (Exception ex)
                    {
                        // 복사 실패 시 메시지 표시
                        MessageBox.Show("복사 실패: " + ex.Message);
                    }
                }

                // 상태 메시지 변경
                SetStatus("복사 완료");
            }
        }

        // 이동 버튼 클릭 이벤트
        private void btnMove_Click(object sender, EventArgs e)
        {
            // 선택된 파일이 없으면 함수 종료
            if (listViewFiles.SelectedItems.Count == 0)
                return;

            // 이동할 대상 폴더를 선택하는 창 생성
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                // 폴더 선택 창에 표시할 설명 설정
                dlg.Description = "이동할 폴더 선택";

                // 사용자가 폴더를 선택하지 않으면 함수 종료
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                // 선택된 파일들을 하나씩 반복
                foreach (ListViewItem item in listViewFiles.SelectedItems)
                {
                    // 항목의 Tag에서 실제 파일 경로 가져오기
                    string src = item.Tag.ToString();

                    try
                    {
                        // FileService를 통해 파일 이동
                        FileService.MoveFile(src, dlg.SelectedPath);
                    }
                    catch (Exception ex)
                    {
                        // 이동 실패 시 메시지 표시
                        MessageBox.Show("이동 실패: " + ex.Message);
                    }
                }

                // 이동 후 파일 목록 다시 불러오기
                LoadFiles();

                // 상태 메시지 변경
                SetStatus("이동 완료");
            }
        }

        // ========================================
        // 자동 정리 버튼 클릭 이벤트
        // - 현재 폴더(_currentFolder)의 파일들을 분석하여
        //   확장자 기반으로 카테고리를 정한 뒤,
        //   필요한 분류 폴더를 자동 생성하고 파일을 이동시킨다.
        // - 사용자에게 미리보기 다이얼로그를 보여준 뒤 실행한다.
        // ========================================
        private void btnAutoOrganize_Click(object sender, EventArgs e)
        {
            try
            {
                // 현재 폴더가 비어있거나 존재하지 않으면 안내
                if (string.IsNullOrEmpty(_currentFolder) || !Directory.Exists(_currentFolder))
                {
                    MessageBox.Show("정리할 폴더가 지정되어 있지 않거나 존재하지 않습니다.",
                        "자동 정리", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 정리 계획 생성 (확장자별 분류 → 대상 폴더/파일명 결정)
                List<OrganizePlanItem> plan = AutoOrganizeService.BuildPlan(_allFiles, _currentFolder);

                // 정리할 파일이 하나도 없을 때 안내 후 종료
                if (plan.Count == 0)
                {
                    MessageBox.Show("정리할 파일이 없습니다.\n(이미 분류 폴더에 들어있거나, 폴더가 비어있습니다)",
                        "자동 정리", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 미리보기 Form을 띄워 사용자 확인을 받음
                using (AutoOrganizeForm dlg = new AutoOrganizeForm(plan))
                {
                    DialogResult dr = dlg.ShowDialog(this);

                    // 사용자가 취소했거나 결과가 없으면 종료
                    if (dr != DialogResult.OK || dlg.Result == null)
                    {
                        SetStatus("자동 정리 취소됨");
                        return;
                    }

                    // 정리 후 파일 목록 갱신 및 상태 메시지 표시
                    LoadFiles();
                    SetStatus("자동 정리 완료: 성공 " + dlg.Result.SuccessCount
                        + "개, 실패 " + dlg.Result.FailedCount
                        + "개, 건너뜀 " + dlg.Result.SkippedCount + "개");

                    // ────────────────────────────────────────
                    // 개선 2: 되돌리기를 위해 결과 보관 + 되돌리기 버튼 활성화
                    // ────────────────────────────────────────
                    if (dlg.Result.SuccessCount > 0)
                    {
                        _lastOrganizeResult = dlg.Result;
                        btnUndo.Enabled = true;
                    }

                    // ────────────────────────────────────────
                    // 개선 3: 정리 직후이므로 알림 배너는 숨김
                    // ────────────────────────────────────────
                    HideNotification();
                }
            }
            catch (Exception ex)
            {
                // 예외 발생 시 사용자에게 알림
                MessageBox.Show("자동 정리 실패: " + ex.Message,
                    "자동 정리 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================
        // 개선 2: 되돌리기 버튼 클릭 이벤트
        //  - 직전 자동 정리의 결과(_lastOrganizeResult)를 역순으로 복원
        //  - 파일을 원래 위치로 되돌리고, 비어 있는 카테고리 폴더는 함께 삭제
        // ========================================
        private void btnUndo_Click(object sender, EventArgs e)
        {
            // 되돌릴 결과가 없으면 종료 (이론상 버튼이 비활성 상태일 때만 발생)
            if (_lastOrganizeResult == null || _lastOrganizeResult.ExecutedMoves.Count == 0)
            {
                MessageBox.Show("되돌릴 작업이 없습니다.",
                    "되돌리기", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 사용자에게 한 번 더 확인
            int moveCount = _lastOrganizeResult.ExecutedMoves.Count;
            DialogResult dr = MessageBox.Show(
                "직전 자동 정리에서 이동된 " + moveCount + "개 파일을 원래 위치로 되돌립니다.\n계속하시겠습니까?",
                "되돌리기", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes)
                return;

            try
            {
                // 마우스 커서를 대기 상태로
                this.Cursor = Cursors.WaitCursor;

                // 되돌리기 실행
                AutoOrganizeResult undoResult = AutoOrganizeService.Undo(_lastOrganizeResult);

                // 커서 복원
                this.Cursor = Cursors.Default;

                // 되돌리기 후에는 더 이상 되돌릴 게 없으므로 버튼 비활성화
                _lastOrganizeResult = null;
                btnUndo.Enabled = false;

                // 결과 메시지
                string msg = "되돌리기가 완료되었습니다.\n\n"
                    + " • 복원: " + undoResult.SuccessCount + "개\n"
                    + " • 실패: " + undoResult.FailedCount + "개\n"
                    + " • 건너뜀: " + undoResult.SkippedCount + "개";

                if (undoResult.FailedCount > 0 && undoResult.Errors.Count > 0)
                    msg += "\n\n[오류 예시]\n" + undoResult.Errors[0];

                MessageBox.Show(msg, "되돌리기 결과",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 파일 목록 새로고침
                LoadFiles();
                SetStatus("되돌리기 완료: 복원 " + undoResult.SuccessCount + "개");
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("되돌리기 중 오류가 발생했습니다.\n" + ex.Message,
                    "되돌리기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================
        // 피드백 ②: 5초 간격 자동 새로고침 Timer
        //  - 폴더 내용이 외부에서 변경되면(새 파일 추가/삭제 등) ListView 자동 갱신
        //  - 절대 파일을 이동시키지 않음 (목록 갱신만)
        //  - LastWriteTime 비교로 실제 변화가 있을 때만 갱신 (불필요한 깜빡임 방지)
        // ========================================
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 현재 폴더가 유효하지 않으면 스킵
                if (string.IsNullOrEmpty(_currentFolder) || !Directory.Exists(_currentFolder))
                    return;

                // 폴더의 현재 LastWriteTime
                DateTime nowWriteTime = new DirectoryInfo(_currentFolder).LastWriteTime;

                // 변화가 없으면 갱신 스킵 (CPU/I/O 절약)
                if (nowWriteTime == _lastFolderWriteTime)
                    return;

                // ──── 사용자 조작 방해 방지 가드 ────
                //   1. 사용자가 항목을 선택 중이면 갱신 보류 (선택이 사라지면 혼란)
                //   2. 사용자가 검색창에 입력 중이면 보류
                //   3. 컨텍스트 메뉴가 열려 있으면 보류
                if (listViewFiles.SelectedItems.Count > 0)
                    return;
                if (txtSearch.Focused)
                    return;
                if (contextMenu != null && contextMenu.Visible)
                    return;

                // 모든 조건 통과 → ListView만 새로고침 (파일 이동 없음)
                LoadFiles();
                SetStatus("화면 자동 갱신됨 (" + DateTime.Now.ToString("HH:mm:ss") + ")");
            }
            catch
            {
                // 백그라운드 작업이므로 예외는 조용히 무시
            }
        }

        // ========================================
        // 개선 3: 30초 간격 정리 대상 감지 Timer
        //  - 정리 대상이 있는지 BuildPlan으로 확인 (실제 이동 없음)
        //  - 있으면 상단 알림 배너만 표시
        //  - 실제 정리는 사용자가 "지금 정리" 버튼을 눌렀을 때만 실행 (피드백 ②)
        // ========================================
        private void ScanTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 폴더가 유효하지 않으면 스캔 스킵
                if (string.IsNullOrEmpty(_currentFolder) || !Directory.Exists(_currentFolder))
                    return;

                // 디스크에서 현재 파일 목록을 다시 읽어 (Form1의 _allFiles가 stale일 수 있으므로)
                List<FileInfo> currentFiles = FileService.LoadFiles(_currentFolder);

                // 정리 계획을 만들어 대상 개수 확인
                List<OrganizePlanItem> plan = AutoOrganizeService.BuildPlan(currentFiles, _currentFolder);
                int targetCount = plan.Count;

                // 정리 대상이 0이면 배너 숨김
                if (targetCount == 0)
                {
                    HideNotification();
                    _lastNotifiedCount = -1;
                    return;
                }

                // 이미 같은 개수로 알림 중이라면 중복 알림 안 함
                if (targetCount == _lastNotifiedCount && _isNotificationVisible)
                    return;

                // 사용자가 정리 다이얼로그를 띄워 둔 동안에는 알림하지 않음
                // (Form1이 비활성화돼 있을 때)
                if (!this.CanFocus)
                    return;

                // 알림 배너 표시
                ShowNotification(targetCount);
                _lastNotifiedCount = targetCount;
            }
            catch
            {
                // 백그라운드 작업이므로 예외는 조용히 무시
            }
        }

        // ========================================
        // 개선 3: 알림 배너 표시 헬퍼
        //  - panelNotification 영역의 라벨 텍스트를 갱신하고 보이기 처리
        // ========================================
        private void ShowNotification(int targetCount)
        {
            // 배너 텍스트 갱신
            lblNotification.Text = "🔔 새로 추가된 정리 가능 파일이 " + targetCount + "개 있습니다. ";

            // 배너를 보이게 처리
            panelNotification.Visible = true;
            _isNotificationVisible = true;
        }

        // 알림 배너 숨김 헬퍼
        private void HideNotification()
        {
            panelNotification.Visible = false;
            _isNotificationVisible = false;
        }

        // 알림 배너의 "지금 정리" 버튼 클릭 이벤트
        private void btnNotifyOrganize_Click(object sender, EventArgs e)
        {
            // 배너 숨기고 자동 정리 다이얼로그 즉시 실행
            HideNotification();
            btnAutoOrganize_Click(sender, e);
        }

        // 알림 배너의 "닫기" 버튼 클릭 이벤트
        private void btnNotifyClose_Click(object sender, EventArgs e)
        {
            // 배너 숨김. 단, _lastNotifiedCount는 유지하여
            // 새 파일이 더 들어오기 전까지는 재알림하지 않음
            HideNotification();
        }

        // 파일 속성 메뉴 클릭 이벤트
        private void menuProperties_Click(object sender, EventArgs e)
        {
            // 속성 보기는 파일 하나만 선택했을 때 가능
            if (listViewFiles.SelectedItems.Count != 1)
                return;

            // 선택된 항목의 실제 파일 경로 가져오기
            string path = listViewFiles.SelectedItems[0].Tag.ToString();

            // 파일이 존재하지 않으면 함수 종료
            if (!File.Exists(path))
                return;

            // 파일 정보를 FileInfo 객체로 생성
            FileInfo fi = new FileInfo(path);

            // 파일 속성 정보를 메시지 박스로 표시
            MessageBox.Show(
                "이름: " + fi.Name + "\n" +
                "크기: " + FileFormatHelper.FormatSize(fi.Length) + "\n" +
                "종류: " + FileCategoryHelper.GetCategory(fi) + "\n" +
                "생성일: " + fi.CreationTime.ToString("yyyy-MM-dd HH:mm") + "\n" +
                "수정일: " + fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm") + "\n" +
                "경로: " + fi.FullName,
                "파일 속성"
            );
        }

        // 미리보기 패널이 다시 그려질 때 실행되는 이벤트
        private void panelPreview_Paint(object sender, PaintEventArgs e)
        {
            // 현재는 별도 그림을 직접 그리지 않으므로 비워둠
        }
    }
}