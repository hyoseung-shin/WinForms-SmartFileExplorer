# AutoOrganize — 전체 코드 해설

**다운로드 폴더 자동 정리 프로그램**(Windows Forms / .NET Framework 4.7.2, 네임스페이스 `Download_file_manager`)을 함수 단위로 설명합니다. 코드를 처음 보는 사람도 따라올 수 있도록, 각 함수마다 *무엇을 하는지* → *어떤 C#/WinForms 기능으로 구현했는지* → *핵심 라인을 한 줄씩* 짚는 순서로 적었습니다.

> 전체 코드는 본인이 작성한 것이므로 핵심 부분은 그대로 인용해 해설합니다. "모든 줄"이 아니라 "중요한 부분을 한 줄씩"이라는 요청에 맞춰, 반복적인 보일러플레이트는 생략하고 구현의 본질이 드러나는 코드만 발췌했습니다.

---

## 1. 큰 그림

이 프로그램은 한 가지 일에 집중하는 작은 "파일 탐색기"입니다. **다운로드 폴더**를 살펴보고, 확장자 기준으로 파일을 분류해서 카테고리 하위 폴더(이미지, 동영상, 문서…)로 옮기는 것 — 그것도 **안전하게**, 그리고 **되돌릴 수 있게** 처리합니다.

코드는 **3개 계층(layer)** 으로 짜여 있고, 이 구조가 전체를 이해하는 가장 중요한 열쇠입니다.

| 계층 | 파일 | 책임 |
|------|------|------|
| **UI (화면 + 컨트롤러)** | `Form1.cs`, `AutoOrganizeForm.cs`, `InputDialog.cs` | 창을 그리고, 버튼 클릭을 처리하고, 대화상자를 띄움. 파일 처리 로직은 *직접 갖지 않고* 서비스 계층에 요청만 함. |
| **Service (로직)** | `FileService.cs`, `AutoOrganizeService.cs`, `PreviewService.cs` | 실제 작업: 폴더 읽기, 파일 이동/복사/삭제, 정리 계획 수립, 되돌리기. UI를 참조하지 않는 `static` 클래스(컨트롤을 파라미터로 받는 경우는 예외). |
| **Helper (순수 규칙)** | `FileCategoryHelper.cs`, `FileFormatHelper.cs` | 작고 상태 없는 판정 함수: "`.png`는 무슨 카테고리?", "2048바이트를 `2.0 KB`로 표시". |

정리 기능 전체를 관통하는 두 가지 설계 아이디어를 머릿속에 넣어 두면 좋습니다.

1. **계획부터 세우고, 그다음에 실행한다(Plan → Execute).** *계획*(이 파일 → 저 폴더 목록)을 먼저 만들어 사용자에게 확인받기 전에는 아무것도 옮기지 않습니다. 일종의 "예행연습(dry-run) → 적용" 분리입니다.
2. **모든 이동은 기록되어 되돌릴 수 있다.** 성공한 이동마다 이동 전/후 경로를 저장하므로, "되돌리기"는 그 목록을 역순으로 재생하기만 하면 됩니다.

여기에 세 번째 주제가 코드 곳곳에 깔려 있습니다. **알림과 이동은 철저히 분리된다.** 백그라운드 타이머는 오직 *살펴보고 알릴* 뿐, 절대 파일을 옮기지 않습니다. 파일은 사용자가 명시적으로 확인했을 때만 움직입니다.

### 전체 흐름

```
Program.Main()                      ← 앱 시작점
   └─ new Form1()                   ← 창 생성, 타이머 2개 시작, 다운로드 폴더 로드
        ├─ NavigateTo(folder)       ← 모든 폴더 이동이 거치는 깔때기
        │     └─ LoadFiles()        ← 디스크 읽기 → ApplyFilterAndSort() → PopulateList() (그리기)
        ├─ "자동 정리" 버튼 클릭
        │     └─ AutoOrganizeService.BuildPlan()        ← 이동 계획 결정 (디스크 변경 없음)
        │           └─ AutoOrganizeForm (미리보기 대화상자)
        │                 └─ AutoOrganizeService.Execute()  ← 실제 이동 + 기록
        └─ "되돌리기" 버튼 클릭
              └─ AutoOrganizeService.Undo()              ← 이동 내역을 역순 재생
```

---

## 2. 진입점 — `Program.cs`

표준적인 WinForms 부팅 코드로, 메인 창을 띄우기만 합니다.

```csharp
[STAThread]
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new Form1());
}
```

- **`[STAThread]`** 는 진입 스레드를 *단일 스레드 아파트(Single-Threaded Apartment)* 로 표시합니다. 클립보드·드래그앤드롭·대화상자 같은 Windows UI 컨트롤이 이를 요구하므로 WinForms에서는 필수입니다.
- **`Application.EnableVisualStyles()`** 는 옛날 Windows 95 식 평면 모양이 아니라 현대적인(테마가 적용된) 컨트롤 렌더링을 켭니다.
- **`Application.Run(new Form1())`** 은 메인 폼을 만들고 *메시지 루프*(창이 닫힐 때까지 마우스/키보드 이벤트를 계속 받는 루프)를 시작합니다. 창이 닫힐 때까지 이 호출이 멈춰 있으므로 마지막 줄에 옵니다.

---

## 3. Helper 계층 — 순수 규칙

### 3.1 `FileCategoryHelper` — "이 파일은 무슨 종류인가?"

핵심은 확장자 → 카테고리 이름 매핑 테이블입니다.

```csharp
public const string CategoryEtc = "기타";   // 분류되지 않은 파일이 가는 폴더

private static readonly Dictionary<string, string> ExtMap =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    {".jpg","이미지"}, {".png","이미지"}, ... {".mp4","동영상"}, ...
    {".pdf","문서"}, ... {".zip","압축파일"}, ... {".exe","실행파일"}, ...
};
```

- **`Dictionary<string,string>`** 로 확장자 → 카테고리를 O(1)에 조회합니다.
- 결정적인 부분은 생성자 인자 **`StringComparer.OrdinalIgnoreCase`** 입니다. 이것이 키를 대소문자 구분 없이 다루게 해 주어 `.JPG`, `.Jpg`, `.jpg` 가 모두 "이미지"로 매핑됩니다 — 모든 대소문자 조합을 따로 저장할 필요가 없죠. `FileInfo.Extension` 은 디스크의 원래 대소문자를 그대로 보존하기 때문에 이 처리가 중요합니다.
- **`const string CategoryEtc`** 는 "기타" 폴더 이름의 단일 진실 공급원(single source of truth)으로, 아래 두 군데에서 재사용됩니다.

**`GetCategory(FileInfo fi)`** — 카테고리를 반환하고, 없으면 "기타"로:

```csharp
return ExtMap.TryGetValue(fi.Extension, out cat) ? cat : CategoryEtc;
```

`TryGetValue` 는 "찾아보되, 없으면 예외를 던지지 말라"는 관용적 패턴입니다. `true`/`false` 를 반환하면서 값은 `out cat` 으로 내보냅니다. 삼항 연산자가 이를 *찾음 → 카테고리, 못 찾음 → 기타* 로 압축합니다.

**`GetAllCategories()`** — 정리 시 만들 수 있는 모든 폴더 이름 목록:

```csharp
List<string> categories = ExtMap.Values.Distinct().ToList();
if (!categories.Contains(CategoryEtc)) categories.Add(CategoryEtc);
return categories;
```

- **`ExtMap.Values`** 는 모든 카테고리 문자열인데, "이미지"를 여러 확장자가 공유하므로 중복이 있습니다.
- **`.Distinct()`** (LINQ) 가 중복을 제거해 카테고리당 하나만 남깁니다.
- "기타"는 `ExtMap` 에 반드시 들어 있지 않은 폴백이므로, 없으면 뒤에 추가합니다.

이 목록은 나중에 *이미 정리된* 폴더(이미지/, 동영상/ …)를 인식하는 데 쓰여, 이미 그 안에 들어 있는 파일을 다시 정리하지 않게 막아 줍니다.

### 3.2 `FileFormatHelper` — 표시 형식 변환

**`FormatSize(long bytes)`** — 바이트 → 사람이 읽기 좋은 문자열, 임계값 캐스케이드로:

```csharp
if (bytes < 1024)              return bytes + " B";
if (bytes < 1024 * 1024)       return (bytes / 1024.0).ToString("F1") + " KB";
if (bytes < 1024L * 1024 * 1024) return (bytes / (1024.0 * 1024)).ToString("F1") + " MB";
return (bytes / (1024.0 * 1024 * 1024)).ToString("F2") + " GB";
```

- 검사는 작은 단위부터 차례로 하고, 가장 먼저 통과한 조건이 이깁니다 — 깔끔하고 분기 비용도 낮습니다.
- **`/ 1024.0`** (`.0` 에 주목) 은 *부동소수점* 나눗셈을 강제합니다. 정수 나눗셈이면 소수부가 잘려 나갑니다.
- **`.ToString("F1")`** 은 .NET *표준 숫자 형식 문자열*로, 소수 1자리 고정소수점(`2.0 KB`)을 뜻합니다. GB는 더 세밀하게 `"F2"` 를 씁니다.
- GB 검사의 **`1024L`** 은 사소해 보여도 실제 버그를 막는 장치입니다. `L`(long) 접미사가 없으면 `1024 * 1024 * 1024` 가 32비트 `int` 로 계산되어 **오버플로**(int 최댓값 초과)가 납니다. `L` 이 연산을 64비트로 승격시킵니다.

**`GetDateGroup(...)`** — 파일이 어느 날짜 그룹(오늘 / 어제 / 이번 주 / 지난 주 / 지난 달 / 오래 전)에 속하는지 고릅니다. 그룹들은 `ListViewGroup` 객체로 받아서, 이 헬퍼가 *어떤* 그룹이 존재하는지에 대해 UI에 독립적으로 유지됩니다.

```csharp
DateTime now  = DateTime.Now.Date;   // 시각 부분을 떼어냄
DateTime date = fileDate.Date;
int diffDays = (now - date).Days;    // TimeSpan.Days = 정수 일 단위 차이

if (diffDays == 0) return today;
if (diffDays == 1) return yesterday;
if (diffDays <= 7) return thisWeek;
if (diffDays <= 14) return lastWeek;
if (diffDays <= 31) return lastMonth;
return older;
```

- **`.Date`** 는 시각 부분을 0으로 만들어 "어제 23:59"와 "오늘 00:01"이 2분이 아니라 하루 차이로 비교되게 합니다.
- **`(now - date)`** 는 두 `DateTime` 을 빼서 **`TimeSpan`** 을 만들고, `.Days` 가 정수 일수입니다. 오름차순 임계값 캐스케이드가 자연스럽게 읽힙니다.

---

## 4. Service 계층 — 실제 작업

### 4.1 `FileService` — 원시 파일시스템 연산

`System.IO` 를 감싼 `static` 도구상자입니다. 핵심만 보면:

**`GetDownloadsPath()`** — 다운로드 폴더를 찾고, 없으면 바탕화면으로 폴백:

```csharp
string path = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
return Directory.Exists(path) ? path
                              : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
```

- **`Environment.GetFolderPath(SpecialFolder.UserProfile)`** 가 `C:\Users\<사용자>` 를 하드코딩 없이 Windows에 물어봅니다.
- **`Path.Combine`** 은 올바른 구분자로 경로 조각을 잇습니다(`\` 를 문자열로 직접 붙이는 오류 위험한 방식을 피함).
- 다운로드 폴더가 없으면 충돌 대신 바탕화면으로 격하됩니다.

**`LoadFiles(folderPath)`** — 파일을 최신순으로 나열:

```csharp
return new DirectoryInfo(folderPath)
    .GetFiles()
    .OrderByDescending(f => f.LastWriteTime)
    .ToList();
```

- **`DirectoryInfo.GetFiles()`** 는 `FileInfo[]`(이름·크기·날짜를 담은 객체 배열)를 반환합니다.
- **`OrderByDescending(f => f.LastWriteTime)`** (LINQ) 가 가장 최근에 수정된 파일을 위로 정렬합니다.
- **`.ToList()`** 가 쿼리를 앱이 들고 다닐 구체적인 `List<FileInfo>` 로 실체화합니다.

**`LoadFolders(folderPath)`** — 보이는 하위 폴더 나열(정리로 만들어진 카테고리 폴더를 표시하는 데 사용):

```csharp
if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
    return new List<DirectoryInfo>();           // 가드 절 → 안전하게 빈 결과

return new DirectoryInfo(folderPath)
    .GetDirectories()
    .Where(d => (d.Attributes & FileAttributes.Hidden) == 0)   // 숨김 폴더 제외
    .OrderBy(d => d.Name)
    .ToList();
```

- 눈여겨볼 부분은 **`(d.Attributes & FileAttributes.Hidden) == 0`** 입니다. `Attributes` 는 *비트 플래그* 열거형이며, `Hidden` 과의 비트 AND(`&`)가 그 한 비트만 분리합니다. `== 0` 은 "숨김 비트가 **꺼져** 있다", 즉 숨김이 아닌 폴더만 남긴다는 뜻입니다. 단일 플래그를 검사하는 정석 기법입니다.

나머지 메서드들은 `System.IO` 위에 얹은 의도적으로 얇은 한 줄짜리들입니다.

```csharp
Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });  // OpenFile
File.Delete(path);                                                     // DeleteFile
File.Copy(sourcePath, destinationPath, true);                          // CopyFile (덮어쓰기=true)
File.Move(sourcePath, destinationPath);                                // MoveFile
File.Move(oldPath, Path.Combine(folder, newName + ext));               // RenameFile
```

- **`UseShellExecute = true`** 는 파일을 실행 파일로 돌리려 하지 말고 *기본 연결 프로그램*으로 열라고 Windows에 지시합니다(탐색기에서 더블클릭하는 것과 동일).
- **`RenameFile`** 이 영리합니다. "이름 변경"은 같은 폴더 안에서의 `File.Move` 일 뿐입니다. 원래 확장자(`Path.GetExtension`)는 유지하고 이름만 바꾸므로, 사용자가 실수로 파일 형식을 깨뜨릴 수 없습니다.

### 4.2 `PreviewService.UpdatePreview(...)` — 오른쪽 미리보기 패널 채우기

시그니처를 보세요. 네 개의 Label과 PictureBox를 **파라미터로** 받습니다. 서비스는 `Form1` 을 알지 못하고, 폼이 갱신할 컨트롤을 건네줍니다. 덕분에 로직이 결합 없이 테스트 가능해집니다.

```csharp
lblPreviewName.Text = "이름: " + fi.Name;
lblPreviewSize.Text = "크기: " + FileFormatHelper.FormatSize(fi.Length);   // 헬퍼 재사용
lblPreviewDate.Text = "날짜: " + fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
lblPreviewPath.Text = "경로: " + fi.FullName;

pictureBoxPreview.Image = null;                 // 이전 이미지를 먼저 비움
string ext = fi.Extension.ToLower();
if (ext == ".jpg" || ext == ".png" || ...)      // 이미지 확장자만
{
    try   { pictureBoxPreview.Image = Image.FromFile(fi.FullName); }
    catch { pictureBoxPreview.Image = null; }   // 손상/잠긴 파일 → 그냥 빈 화면
}
```

- 크기를 다시 포맷하지 않고 `FileFormatHelper.FormatSize` 를 재사용합니다 — 로직 중복이 없습니다.
- **`Image.FromFile`** 은 비트맵을 로드하는데, 파일이 손상되었거나 잠겨 있으면 예외를 던질 수 있어 `try/catch` 로 감싸 조용히 빈 미리보기로 폴백합니다. 미리보기가 앱을 죽여서는 안 되니까요.

### 4.3 `InputDialog.Show(title, prompt, defaultValue)` — 직접 만든 입력창

WinForms에는 "문자열을 입력받는" 기본 대화상자가 없으므로, *코드로 직접* 하나 만듭니다(디자이너 파일 없음). 입력 문자열을 반환하고, 취소하면 `null` 을 돌려줍니다. 이름 변경 기능에서 사용됩니다.

생성 과정 자체는 기계적입니다(`Form`, `Label`, `TextBox`, 버튼 2개를 만들고 위치 지정). 이것을 진짜 대화상자처럼 동작하게 만드는 부분은:

```csharp
ok.DialogResult     = DialogResult.OK;       // 확인 클릭 시 폼이 닫히고 ShowDialog가 OK 반환
cancel.DialogResult = DialogResult.Cancel;   // 취소 클릭 시 Cancel 반환
...
form.AcceptButton = ok;       // Enter 키  → 확인 버튼 동작
form.CancelButton = cancel;   // Esc 키    → 취소 버튼 동작

return form.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
```

- 버튼에 **`DialogResult` 를 할당**하는 것이 핵심 트릭입니다. 이렇게 하면 버튼이 모달 폼을 자동으로 닫고 *왜* 닫혔는지 보고합니다 — 수동 `Close()` 가 필요 없습니다.
- **`AcceptButton` / `CancelButton`** 은 Enter/Esc를 해당 버튼에 연결합니다 — 사용자가 기대하는 동작이죠.
- **`ShowDialog()`** 는 폼을 *모달*로(닫힐 때까지 부모를 막음) 열고 닫힐 때의 `DialogResult` 를 반환합니다. 마지막 삼항 연산자가 전체 상호작용을 "확인 → 다듬은 텍스트, 그 외 → null" 로 압축하므로, 호출자는 `null` 인지만 검사하면 됩니다.

### 4.4 `AutoOrganizeService` — 앱의 심장

정리/되돌리기 로직을 담은 static 클래스입니다. 먼저 작은 **데이터 운반 클래스 3개**(필드만 있는 평범한 객체):

```csharp
public class OrganizePlanItem {           // 계획된 이동 1건
    public FileInfo Source;               //   옮길 파일
    public string CategoryName;           //   "이미지", "문서", ...
    public string TargetFolderPath;       //   ...\Downloads\이미지
    public string TargetFullPath;         //   최종 경로 (충돌 시 이름이 바뀔 수 있음)
    public bool   IsRenamed;              //   "(1)" 접미사가 붙었으면 true
}

public class MoveRecord {                 // *완료된* 이동 1건 (되돌리기용)
    public string OriginalPath;           //   원래 있던 곳
    public string MovedPath;              //   옮겨간 곳
    public string CreatedFolder;          //   이 이동이 만든 폴더, 아니면 null
}

public class AutoOrganizeResult {         // 한 번 실행의 결과
    public int SuccessCount, FailedCount, SkippedCount;
    public Dictionary<string,int> CategoryCounts = new ...;   // "이미지" → 5
    public List<string> Errors = new ...;
    public List<string> CreatedFolders = new ...;
    public List<MoveRecord> ExecutedMoves = new ...;          // ← Undo가 재생하는 대상
    public DateTime ExecutedAt = DateTime.Now;
}
```

분리가 의도적입니다. `OrganizePlanItem` 은 *의도*, `MoveRecord` 는 *영수증*입니다. 되돌리기는 영수증을 기반으로 동작하므로, 실제로 일어난 일만 되돌릴 수 있습니다.

#### `BuildPlan(files, baseFolder)` — 이동을 결정 (디스크는 건드리지 않음)

예행연습 단계입니다. 미리보기 대화상자가 보여줄 `List<OrganizePlanItem>` 을 만듭니다.

```csharp
List<OrganizePlanItem> plan = new List<OrganizePlanItem>();
if (files == null || files.Count == 0 || string.IsNullOrEmpty(baseFolder))
    return plan;                                  // 가드: 할 일 없음 → 빈 계획

HashSet<string> categoryFolderSet = BuildCategoryFolderSet(baseFolder);

foreach (FileInfo fi in files)
{
    if (fi == null || !fi.Exists) continue;       // 유령 항목 건너뜀

    string parentFolder = Path.GetFullPath(fi.DirectoryName);
    if (categoryFolderSet.Contains(parentFolder)) // 이미 이미지/, 동영상/, ... 안에 있음
        continue;                                 //   → 다시 정리하지 않음

    string category    = FileCategoryHelper.GetCategory(fi);     // 헬퍼 재사용
    string targetFolder= Path.Combine(baseFolder, category);
    string candidatePath = Path.Combine(targetFolder, fi.Name);
    string finalPath   = ResolveCollisionName(targetFolder, fi.Name, plan);  // 이름 중복 해소
    bool   renamed     = !string.Equals(finalPath, candidatePath, StringComparison.OrdinalIgnoreCase);

    plan.Add(new OrganizePlanItem {
        Source = fi, CategoryName = category,
        TargetFolderPath = targetFolder, TargetFullPath = finalPath, IsRenamed = renamed
    });
}

return plan.OrderBy(p => p.CategoryName).ThenBy(p => p.Source.Name).ToList();
```

핵심 기능과 *그 이유*:

- **가드 절 먼저.** 잘못된 입력은 예외 대신 빈 계획을 반환합니다 — 여기 모든 public 메서드는 방어적입니다.
- **`HashSet<string>`(`categoryFolderSet`)** — 루프 전에 한 번만 만듭니다. 멤버십 검사가 O(1)이라 "이 파일이 이미 카테고리 폴더 안에 있나?"를 수천 개 파일에도 싸게 검사합니다. 이 검사로 *이미 정리된* 파일을 건너뛰는 것이 정리를 안전하고 멱등(idempotent)하게 만듭니다.
- **`Path.GetFullPath(...)`** 는 경로를 정규화(`..`, 대소문자, 끝 슬래시 해소)해서 `HashSet.Contains` 비교를 신뢰할 수 있게 합니다.
- **`ResolveCollisionName`** 이 충돌하지 않는 대상 이름을 계산합니다(아래 참조). `IsRenamed` 는 접미사가 필요했는지를 기록하는데, 순전히 미리보기에서 `*` 로 표시하기 위함입니다.
- **`OrderBy(category).ThenBy(name)`** — 최종 정렬로 미리보기를 보기 좋게 묶습니다(문서 끼리 모이고, 그 안에서 가나다순). `ThenBy` 는 LINQ의 2차 정렬 키입니다.

#### `Execute(plan)` — 실제로 이동 수행

```csharp
AutoOrganizeResult result = new AutoOrganizeResult();
if (plan == null || plan.Count == 0) return result;

HashSet<string> createdSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

foreach (OrganizePlanItem item in plan)
{
    try
    {
        if (item.Source == null || !File.Exists(item.Source.FullName))
        { result.SkippedCount++; continue; }                    // 파일이 사라짐 → 건너뜀

        if (string.Equals(Path.GetFullPath(item.Source.FullName),
                          Path.GetFullPath(item.TargetFullPath),
                          StringComparison.OrdinalIgnoreCase))
        { result.SkippedCount++; continue; }                    // 이미 제자리 → 건너뜀

        bool createdNow = false;
        if (!Directory.Exists(item.TargetFolderPath))
        {
            Directory.CreateDirectory(item.TargetFolderPath);   // 카테고리 폴더 자동 생성
            if (createdSet.Add(item.TargetFolderPath))          // .Add는 중복이면 false 반환
            { result.CreatedFolders.Add(item.TargetFolderPath); createdNow = true; }
        }

        string originalPath = item.Source.FullName;             // 이동 전에 미리 보관
        File.Move(originalPath, item.TargetFullPath);           // 실제 이동

        result.ExecutedMoves.Add(new MoveRecord {               // "영수증" 작성
            OriginalPath = originalPath,
            MovedPath    = item.TargetFullPath,
            CreatedFolder= createdNow ? item.TargetFolderPath : null
        });

        result.SuccessCount++;
        if (result.CategoryCounts.ContainsKey(item.CategoryName))
            result.CategoryCounts[item.CategoryName]++;         // 증가-또는-초기화 패턴
        else
            result.CategoryCounts[item.CategoryName] = 1;
    }
    catch (Exception ex)
    {
        result.FailedCount++;
        result.Errors.Add(item.Source.Name + " → " + ex.Message);  // 중단 말고 기록만
    }
}
return result;
```

이 코드가 견고한 이유:

- **항목별 `try/catch`** 가 실패를 격리합니다. 7번 파일이 다른 프로그램에 잠겨 있어도, 8~N번 파일은 계속 정리됩니다. 오류는 `Errors` 에 기록되고 `FailedCount` 로 집계됩니다. 파일 하나가 배치 전체를 중단시키는 일은 없습니다.
- **두 가지 건너뛰기 조건**이 `Execute` 를 멱등하게 만듭니다. 사라진 파일, 또는 원본이 이미 대상과 같은 파일은 *건너뜀*으로 세고 그대로 둡니다.
- **`Directory.CreateDirectory`** 자체도 멱등(이미 있으면 오류 없음)이지만, 코드는 여전히 `!Directory.Exists` 로 감쌉니다 — *이번 실행*이 폴더를 만들었는지 추적하기 위함입니다. 이것을 `createdNow` 와 `MoveRecord.CreatedFolder` 가 포착해, 나중에 Undo가 자신이 만든 폴더를 지울 수 있게 합니다.
- **`createdSet.Add(...)` 가 bool을 반환**하는 점을 중복 제거에 활용합니다. `HashSet.Add` 는 이미 있으면 `false` 를 반환하므로, 여러 파일이 같은 카테고리 폴더를 향해도 `CreatedFolders` 에는 정확히 한 번만 추가됩니다.
- **`File.Move` 전에 `originalPath` 를 포착**하는 게 중요합니다. 이동 후에는 `item.Source.FullName` 이 새 위치를 가리키기 때문에, 되돌리기를 가능하게 하려면 원래 경로를 먼저 저장해야 합니다.

#### `ResolveCollisionName(targetFolder, fileName, existingPlan)` — 절대 덮어쓰지 않기

```csharp
string candidate = Path.Combine(targetFolder, fileName);
if (!File.Exists(candidate) && !PlanContainsTarget(existingPlan, candidate))
    return candidate;                                   // 충돌 없음 → 그대로 사용

string nameOnly = Path.GetFileNameWithoutExtension(fileName);
string ext      = Path.GetExtension(fileName);
int n = 1;
while (true)
{
    string newPath = Path.Combine(targetFolder, nameOnly + " (" + n + ")" + ext);
    if (!File.Exists(newPath) && !PlanContainsTarget(existingPlan, newPath))
        return newPath;                                 // "photo (1).jpg", "photo (2).jpg", ...
    n++;
    if (n > 100000) return newPath;                     // 이론상 무한루프 방지용
}
```

핵심은 충돌을 **두 군데**에서 검사한다는 점입니다.
1. **`File.Exists`** — 같은 이름의 파일이 이미 디스크에 있음.
2. **`PlanContainsTarget`** — *같은 배치 안의* 다른 파일이 이미 그 이름으로 향하고 있음(예: 서로 다른 하위 폴더의 `report.pdf` 두 개). 이 검사가 없으면 두 번째 이동이 첫 번째를 덮어씁니다.

두 검사가 모두 통과할 때까지 ` (1)`, ` (2)`… 를 증가시킵니다. `n > 100000` break는 일어날 수 없는 무한루프에 대한 편집증적 안전장치입니다.

#### `PlanContainsTarget`, `BuildCategoryFolderSet`, `SummarizePlan` — 작은 헬퍼들

```csharp
// 계획된 항목 중 이 경로를 향하는 게 있나? (대소문자 무시)
return existingPlan.Any(p => string.Equals(p.TargetFullPath, path, StringComparison.OrdinalIgnoreCase));
```
**`.Any(...)`** (LINQ) 는 첫 일치에서 `true` 로 단락(short-circuit)합니다.

```csharp
// 가능한 모든 카테고리 폴더의 절대 경로 → "이미 정리됨" 집합
foreach (string cat in FileCategoryHelper.GetAllCategories())
    set.Add(Path.GetFullPath(Path.Combine(baseFolder, cat)));
```

```csharp
// "이미지: 5개\n문서: 12개\n\n 합계: 17개 파일"  — 미리보기 요약 텍스트
var grouped = plan.GroupBy(p => p.CategoryName).OrderBy(g => g.Key).ToList();
foreach (var g in grouped) lines.Add(" • " + g.Key + ": " + g.Count() + "개");
return string.Join("\n", lines);
```
**`GroupBy`** 가 카테고리별로 항목을 묶고, **`g.Count()`** 가 각 묶음의 크기이며, **`string.Join("\n", …)`** 가 줄들을 이어 붙입니다.

`OrganizeFolder` 는 둘을 연결한 편의 함수입니다: `Execute(BuildPlan(files, baseFolder))` — 미리보기 없이 정리하고 싶을 때 쓸 수 있습니다.

#### `Undo(original)` — 직전 실행 역전

```csharp
AutoOrganizeResult undoResult = new AutoOrganizeResult();
if (original == null || original.ExecutedMoves == null || original.ExecutedMoves.Count == 0)
    return undoResult;

for (int i = original.ExecutedMoves.Count - 1; i >= 0; i--)   // ← 역순
{
    MoveRecord rec = original.ExecutedMoves[i];
    try
    {
        if (!File.Exists(rec.MovedPath)) { undoResult.SkippedCount++; continue; }

        string restorePath = rec.OriginalPath;
        if (File.Exists(restorePath))           // 원래 이름이 다시 점유됨 → 안전한 이름 생성
        {
            string dir = Path.GetDirectoryName(restorePath);
            string nameOnly = Path.GetFileNameWithoutExtension(restorePath);
            string ext = Path.GetExtension(restorePath);
            int n = 1;
            while (File.Exists(restorePath)) {
                restorePath = Path.Combine(dir, nameOnly + " (복원 " + n + ")" + ext);
                n++; if (n > 100000) break;
            }
        }

        string targetDir = Path.GetDirectoryName(restorePath);
        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);   // 사라졌으면 재생성
        File.Move(rec.MovedPath, restorePath);                                    // 원위치로 이동
        undoResult.SuccessCount++;

        if (!string.IsNullOrEmpty(rec.CreatedFolder) && Directory.Exists(rec.CreatedFolder))
        {
            bool isEmpty = Directory.GetFiles(rec.CreatedFolder).Length == 0
                        && Directory.GetDirectories(rec.CreatedFolder).Length == 0;
            if (isEmpty) { try { Directory.Delete(rec.CreatedFolder); } catch { } }  // 빈 폴더 정리
        }
    }
    catch (Exception ex) { undoResult.FailedCount++; undoResult.Errors.Add(...); }
}
```

영리한 부분들:

- **역순 순회**(`i = Count-1; i >= 0; i--`). 이동을 마지막 것부터 거꾸로 재생합니다. 이것이 중첩 효과에 맞는 올바른 순서입니다 — 폴더를 지우려 시도하기 전에 폴더가 먼저 비워지니까요.
- **되돌리기 자체를 위한 *새* `AutoOrganizeResult`** 를 만들어, 되돌리기의 성공/실패 카운트를 별도로 보고합니다.
- **복원 이름 충돌 처리**는 `ResolveCollisionName` 을 거울처럼 따르되 ` (복원 N)` 접미사를 씁니다. 그래서 원래 위치에 그새 다시 생긴 파일을 절대 덮어쓰지 않습니다.
- **부모 폴더가 사라졌으면 재생성**한 뒤 `File.Move` 로 돌려놓습니다.
- **빈 폴더 정리**: *이번 실행이 만든*(`rec.CreatedFolder != null`) 폴더이면서 *지금 비어 있는* 폴더만 `GetFiles().Length == 0 && GetDirectories().Length == 0` 검사를 통과해 삭제됩니다. 안쪽 `try { Delete } catch { }` 덕분에 정리에 실패해도 되돌리기가 깨지지 않습니다.

---

## 5. `AutoOrganizeForm` — 미리보기/확인 대화상자

두 번째 창으로, 전부 코드로 만들어집니다(디자이너 파일 없음). `Form1` 이 이 창을 만들어 계획을 건네고, 다시 `Result` 를 읽어 옵니다.

결과는 **외부에서는 읽기 전용인 속성**으로 노출됩니다.

```csharp
public AutoOrganizeResult Result { get; private set; }   // 외부는 읽기만; 이 클래스만 설정 가능
```

생성자는 계획을 방어적으로 저장하고 UI를 구성합니다.

```csharp
public AutoOrganizeForm(List<OrganizePlanItem> plan)
{
    _plan = plan ?? new List<OrganizePlanItem>();   // null 병합 → 절대 null이 아님
    InitializeUI();
    FillData();
}
```

**`InitializeUI()`** 는 고정 크기 모달 창과 세 컬럼짜리 **Details 모드 `ListView`** 를 세팅합니다.

```csharp
listPreview.View = View.Details;     // 아이콘이 아니라 표 형태의 행/열
listPreview.FullRowSelect = true;
listPreview.Columns.Add("파일 이름", 250);
listPreview.Columns.Add("→ 분류", 80);
listPreview.Columns.Add("→ 이동 후 이름", 240);
...
btnRun.Click += btnRun_Click;        // 버튼을 핸들러에 연결
btnCancel.DialogResult = DialogResult.Cancel;   // 취소가 Cancel로 자동 닫힘
```

**`FillData()`** 는 요약 라벨과 리스트를 카테고리별로 묶어 채웁니다.

```csharp
lblSummary.Text = "다음과 같이 분류 후 이동됩니다.\n\n" + AutoOrganizeService.SummarizePlan(_plan);

var grouped = _plan.GroupBy(p => p.CategoryName).OrderBy(g => g.Key);
listPreview.ShowGroups = true;
foreach (var g in grouped)
{
    ListViewGroup lvGroup = new ListViewGroup(g.Key + " (" + g.Count() + "개)");  // "이미지 (5개)"
    listPreview.Groups.Add(lvGroup);
    foreach (OrganizePlanItem item in g.OrderBy(x => x.Source.Name))
    {
        ListViewItem lvi = new ListViewItem(item.Source.Name);
        lvi.SubItems.Add(item.CategoryName);
        string movedName = Path.GetFileName(item.TargetFullPath);
        if (item.IsRenamed) movedName = "* " + movedName;   // 이름 바뀐 파일 표시
        lvi.SubItems.Add(movedName);
        lvi.Group = lvGroup;
        listPreview.Items.Add(lvi);
    }
}
```

- **`ListViewItem`** 하나가 한 행입니다. 첫 텍스트가 1열이고, `SubItems.Add` 마다 다음 열을 채웁니다.
- **`ListViewGroup`** 이 접을 수 있는 카테고리 헤더를 만들고, `lvi.Group` 을 지정해 행을 그 헤더 아래에 배치합니다.
- **`*` 접두어**(`IsRenamed` 기반)가 이름을 바꿔야 했던 파일을 표시해, 사용자가 충돌을 미리 알 수 있게 합니다.

**`btnRun_Click`** 이 실행을 최종 승인하는 곳입니다.

```csharp
DialogResult dr = MessageBox.Show(_plan.Count + "개의 파일을 분류 폴더로 이동합니다.\n계속하시겠습니까?",
                                  "자동 정리 실행", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
if (dr != DialogResult.Yes) return;     // 아니오 → 창 유지, 아무것도 안 바꿈

this.Cursor = Cursors.WaitCursor;       // 작업 중 모래시계 표시
Result = AutoOrganizeService.Execute(_plan);   // ← Execute가 호출되는 유일한 지점
this.Cursor = Cursors.Default;

MessageBox.Show( /* 성공/실패/건너뜀/생성된 폴더 수 */ );
this.DialogResult = DialogResult.OK;    // 호출자에게 성공을 알림
this.Close();
```

- **`YesNo` 가 달린 `MessageBox.Show`** 가 두 번째 명시적 확인입니다 — 대화상자 자체가 첫 번째 확인이죠. "예" 다음에야 `Execute` 가 돕니다.
- **`this.DialogResult = DialogResult.OK`** 설정이 `Form1` 이 기다리는 신호입니다. 값이 채워진 `Result` 속성과 합쳐져, 두 창 사이의 계약 전체가 됩니다.
- `btnCancel_Click` 은 `Result = null` 로 두고 `Cancel` 로 닫으므로, 호출자가 "취소"와 "실행됨"을 구분할 수 있습니다.

---

## 6. `Form1` — 메인 창과 컨트롤러

가장 큰 파일입니다. 애플리케이션 *상태*를 보관하고 모든 UI 이벤트를 서비스로 라우팅합니다. 위쪽 상태 필드만 봐도 폼이 무엇을 추적하는지 거의 알 수 있습니다.

```csharp
private string _currentFolder;                 // 어떤 폴더가 열려 있나
private List<FileInfo> _allFiles;              // 그 폴더의 파일들 (마스터 목록)
private List<DirectoryInfo> _allFolders;       // 그 폴더의 하위 폴더들
private bool _sortAscending;                   // 정렬 방향 토글
private AutoOrganizeResult _lastOrganizeResult;// 되돌리기용
private Timer _scanTimer;                       // 30초: 정리 가능 파일 감지 → 배너
private Timer _refreshTimer;                    // 5초: 폴더가 바뀌면 목록 자동 갱신
private DateTime _lastFolderWriteTime;          // 5초 타이머의 변경 감지 기준값
private int _lastNotifiedCount;                 // 배너 중복 제거
private bool _isNotificationVisible;
private Stack<string> _backStack;               // 브라우저식 히스토리
private Stack<string> _forwardStack;
```

두 가지 상태 선택이 눈에 띕니다. 뒤로/앞으로 히스토리에 **`Stack<string>`**(스택은 바로 "마지막 방문, 먼저 복귀"에 딱 맞음)을 썼고, 역할이 명확히 분리된 **두 개의 `Timer`** 를 둔 것입니다.

### 6.1 생성자 — 시작 + 타이머

```csharp
InitializeComponent();          // 디자이너에서 정의한 컨트롤 생성
cmbSort.SelectedIndex = 3;      // 기본 정렬 = 수정 날짜
_sortAscending = false;         // 최신순

string downloadsPath = FileService.GetDownloadsPath();
NavigateTo(downloadsPath, addToHistory: false);   // 초기 폴더, 히스토리에는 기록 안 함

_refreshTimer = new System.Windows.Forms.Timer();
_refreshTimer.Interval = 5000;  _refreshTimer.Tick += RefreshTimer_Tick;  _refreshTimer.Start();

_scanTimer = new System.Windows.Forms.Timer();
_scanTimer.Interval = 30000;    _scanTimer.Tick += ScanTimer_Tick;        _scanTimer.Start();

this.FormClosed += (s, e) => {  // 창이 닫힐 때 타이머 정리
    if (_refreshTimer != null) { _refreshTimer.Stop(); _refreshTimer.Dispose(); }
    if (_scanTimer    != null) { _scanTimer.Stop();    _scanTimer.Dispose(); }
};
```

- **`System.Windows.Forms.Timer`** 는 `Tick` 이벤트를 *UI 스레드*에서 발생시킵니다. 핸들러가 컨트롤을 건드리는데 WinForms 컨트롤은 UI 스레드에서만 만질 수 있으므로 이게 중요합니다.(전체 네임스페이스를 쓴 건 무관한 `System.Threading.Timer` 와 혼동을 피하려는 것)
- 두 타이머는 의도적으로 서로 다른 주기의 서로 다른 일입니다. 5초 = 폴더가 바뀌면 목록을 다시 그림, 30초 = 정리할 게 있는지 확인해 배너 표시. **둘 다 파일을 옮기지 않습니다.**
- **`FormClosed += (s, e) => { … }`** 는 두 타이머를 멈추고 dispose하는 *람다* 이벤트 핸들러입니다. 이게 없으면 타이머가 죽은 창을 향해 계속 발화합니다.

### 6.2 네비게이션 시스템 — `NavigateTo` 와 친구들

모든 폴더 이동이 하나의 깔때기 `NavigateTo` 를 거치는데, 이것이 히스토리를 일관되게 유지하는 비결입니다.

```csharp
private void NavigateTo(string targetFolder, bool addToHistory = true)
{
    if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder))
    { SetStatus("폴더를 열 수 없습니다: " + targetFolder); return; }

    if (string.Equals(targetFolder, _currentFolder, StringComparison.OrdinalIgnoreCase))
        return;                                       // 같은 폴더 → 무시 (쓸데없는 히스토리 방지)

    if (addToHistory && !string.IsNullOrEmpty(_currentFolder))
    {
        _backStack.Push(_currentFolder);              // 있던 곳을 기억
        _forwardStack.Clear();                        // 새 분기는 "앞으로"를 무효화
    }

    _currentFolder = targetFolder;
    _lastNotifiedCount = -1;  HideNotification();      // 새 폴더에 맞춰 배너 상태 리셋
    LoadFiles();                                       // 로드 + 렌더링
    UpdateNavigationUI();                              // 주소표시줄 + 버튼 상태 갱신
}
```

- **`addToHistory`(기본 `true`)** 가 *일반* 이동(현재 폴더를 back-stack에 push)과 *히스토리* 이동(뒤로/앞으로/초기 로드, 이때 스택은 호출자가 관리하므로 건드리면 안 됨)을 구분하는 단 하나의 스위치입니다.
- **새 이동 시 `_forwardStack.Clear()`** 가 정확히 브라우저 동작을 재현합니다. 새 곳으로 가면 이전의 "앞으로" 경로는 사라지죠.

세 버튼은 스택을 조작하고 위임만 합니다.

```csharp
// 뒤로: 현재를 forward에 push, 가장 최근 back을 pop, 거기로 이동
_forwardStack.Push(_currentFolder);
NavigateTo(_backStack.Pop(), addToHistory: false);

// 앞으로: 거울상
_backStack.Push(_currentFolder);
NavigateTo(_forwardStack.Pop(), addToHistory: false);

// 위로: 부모 폴더는 *일반* 이동이므로 addToHistory는 true 유지
NavigateTo(Path.GetDirectoryName(_currentFolder), addToHistory: true);
```

**`UpdateNavigationUI()`** 는 버튼을 정직하게 유지합니다 — 갈 곳이 있을 때만 활성화하고 아니면 회색 처리.

```csharp
btnBack.Enabled = _backStack.Count > 0;
btnBack.ForeColor = _backStack.Count > 0 ? Color.FromArgb(30,30,30) : Color.LightGray;
// ... 앞으로/위로도 같은 패턴 ...
lblAddress.Text = "  🖥  >  " + GetDisplayPath(_currentFolder);
```

**`GetDisplayPath`** 는 주소표시줄을 짧게 만듭니다. 다운로드 폴더 안의 경로는 전체 `C:\Users\…\Downloads\이미지` 대신 `Downloads > 이미지` 로 표시합니다 — `Substring` 으로 Downloads 접두어를 떼고 구분자를 치환해서요.

### 6.3 렌더 파이프라인 — `LoadFiles → ApplyFilterAndSort → PopulateList`

각 단계가 하나의 일만 맡는 깔끔한 3단 파이프라인입니다.

**`LoadFiles()` — 디스크에서 마스터 목록으로 읽기:**

```csharp
_allFiles   = FileService.LoadFiles(_currentFolder);
_allFolders = FileService.LoadFolders(_currentFolder);
if (Directory.Exists(_currentFolder))
    _lastFolderWriteTime = new DirectoryInfo(_currentFolder).LastWriteTime;  // 5초 타이머 기준값
ApplyFilterAndSort();    // 2단계로 넘김
UpdateStatus();
```

`_lastFolderWriteTime` 을 기록해 두어, 새로고침 타이머가 나중에 폴더가 바뀌었는지 판별합니다.

**`ApplyFilterAndSort()` — 메모리 목록을 좁히고 정렬(디스크 I/O 없음):**

```csharp
string search = txtSearch.Text.Trim().ToLower();
string filterCat = cmbFilter.SelectedItem?.ToString() ?? "전체";   // null → "전체"

IEnumerable<FileInfo> filtered = _allFiles;
if (!string.IsNullOrEmpty(search))
    filtered = filtered.Where(f => f.Name.ToLower().Contains(search));     // 이름 검색
if (filterCat != "전체")
    filtered = filtered.Where(f => FileCategoryHelper.GetCategory(f) == filterCat);  // 카테고리 필터

if (cmbSort.SelectedIndex == 0)        // 0 = 이름
    filtered = _sortAscending ? filtered.OrderBy(f => f.Name)      : filtered.OrderByDescending(f => f.Name);
else if (cmbSort.SelectedIndex == 1)   // 1 = 크기
    filtered = _sortAscending ? filtered.OrderBy(f => f.Length)    : filtered.OrderByDescending(f => f.Length);
else if (cmbSort.SelectedIndex == 2)   // 2 = 종류
    filtered = _sortAscending ? filtered.OrderBy(f => FileCategoryHelper.GetCategory(f)) : ...;
else if (cmbSort.SelectedIndex == 3)   // 3 = 수정 날짜
    filtered = _sortAscending ? filtered.OrderBy(f => f.LastWriteTime) : filtered.OrderByDescending(...);

PopulateList(filtered.ToList());       // 3단계로 넘김
```

- **`IEnumerable<FileInfo>` + 체이닝된 `Where`** 가 *지연(lazy)* LINQ 쿼리를 쌓습니다 — 마지막 `.ToList()` 전까지는 실제로 아무것도 실행되지 않습니다. 각 필터가 조건부로 덧붙어 깔끔하게 합성됩니다.
- `cmbSort.SelectedIndex` 캐스케이드가 드롭다운을 정렬 키로 매핑하고, `_sortAscending` 이 방향을 뒤집습니다. 이 메서드가 매번 텍스트박스·드롭다운·방향 플래그를 다시 읽기 때문에, 모든 이벤트 핸들러(검색/필터/정렬/방향)는 그냥 `ApplyFilterAndSort()` 만 호출하면 알맞게 동작합니다.

**`PopulateList(files)` — `ListView` 에 그리기:**

```csharp
listViewFiles.BeginUpdate();          // 다시 그리기 보류 → 재구성 중 깜빡임 없음
listViewFiles.Items.Clear();
listViewFiles.Groups.Clear();

ListViewGroup groupFolders = new ListViewGroup("폴더");   // 폴더는 최상단 고정
listViewFiles.Groups.Add(groupFolders);
// ... 6개 날짜 그룹(오늘/어제/이번 주/지난 주/지난 달/오래 전) 생성 후 추가 ...

// 폴더 먼저 (필터가 "전체"이고 검색어와 일치할 때만):
foreach (DirectoryInfo di in _allFolders) {
    if (!string.IsNullOrEmpty(searchLower) && !di.Name.ToLower().Contains(searchLower)) continue;
    ListViewItem folderItem = new ListViewItem("📁 " + di.Name);
    folderItem.SubItems.Add("(폴더)");                     // 크기 열에 "(폴더)" 표시
    folderItem.SubItems.Add("폴더");
    folderItem.SubItems.Add(di.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
    folderItem.Tag = "DIR:" + di.FullName;                 // ← 접두어 마커, 핵심 아이디어
    folderItem.ForeColor = Color.FromArgb(0,90,158);
    folderItem.Font = new Font(listViewFiles.Font, FontStyle.Bold);
    folderItem.Group = groupFolders;
    listViewFiles.Items.Add(folderItem);
    visibleFolderCount++;
}
if (visibleFolderCount == 0) listViewFiles.Groups.Remove(groupFolders);  // 빈 그룹은 숨김

// 그다음 파일들, 각자 날짜 그룹에 배치:
foreach (FileInfo fi in files) {
    ListViewItem item = new ListViewItem(fi.Name);
    item.SubItems.Add(FileFormatHelper.FormatSize(fi.Length));
    item.SubItems.Add(FileCategoryHelper.GetCategory(fi));
    item.SubItems.Add(fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
    item.Tag = fi.FullName;                                // ← 순수 경로 = 파일이라는 뜻
    item.Group = FileFormatHelper.GetDateGroup(fi.LastWriteTime, groupToday, ... groupOlder);
    listViewFiles.Items.Add(item);
}

listViewFiles.ShowGroups = true;
listViewFiles.EndUpdate();             // 다시 그리기 재개 → 한 번에 페인트
```

이 메서드를 지탱하는 두 기법:

- **`BeginUpdate()` / `EndUpdate()`** 가 재구성을 감싸서, `Items.Add` 마다 깜빡이지 않고 마지막에 **한 번** 다시 그립니다. 목록을 비우고 다시 채울 때 필수입니다.
- **`Tag` 컨벤션.** 모든 행이 자기 정체성을 `ListViewItem.Tag` 에 넣습니다. 폴더는 `"DIR:" + 경로`, 파일은 순수 `경로`. 나중에 더블클릭/열기 등이 `Tag` 를 읽고 `"DIR:"` 접두어로 폴더와 파일을 분기합니다. UI 행에 숨은 데이터를 붙이는 가벼운 방법입니다.

### 6.4 선택, 더블클릭, 파일 작업

**선택**은 행의 `Tag` 를 읽어 `PreviewService` 에 위임해 미리보기를 갱신합니다.

```csharp
string path = listViewFiles.SelectedItems[0].Tag.ToString();
if (!File.Exists(path)) return;
PreviewService.UpdatePreview(new FileInfo(path), lblPreviewName, ... pictureBoxPreview);
```

**더블클릭**은 `Tag` 컨벤션으로 *이동 vs 열기*를 결정합니다.

```csharp
string tag = selected.Tag?.ToString() ?? "";
if (tag.StartsWith("DIR:")) {
    NavigateTo(tag.Substring(4), addToHistory: true);  // 폴더 → 진입 (히스토리 증가)
    return;
}
OpenSelected();                                        // 파일 → 기본 프로그램으로 열기
```

**파일 작업 핸들러**(열기, 삭제, 이름 변경, 복사, 이동)는 모두 하나의 일관된 레시피를 따릅니다. 나머지는 변주이므로 한 번만 보면 됩니다.

```csharp
private void btnDelete_Click(object sender, EventArgs e)
{
    if (listViewFiles.SelectedItems.Count == 0) return;          // 1. 선택 없음 → 종료

    DialogResult result = MessageBox.Show("선택한 파일을 삭제하시겠습니까?", "삭제 확인",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
    if (result != DialogResult.Yes) return;                      // 2. 파괴적 작업이면 확인

    foreach (ListViewItem item in listViewFiles.SelectedItems)   // 3. 선택을 루프
    {
        try   { FileService.DeleteFile(item.Tag.ToString()); }   // 4. 서비스에 위임
        catch (Exception ex) { MessageBox.Show("삭제 실패: " + ex.Message); }  // 5. 오류 격리
    }
    LoadFiles();                                                 // 6. 화면 갱신
    SetStatus("삭제 완료");
}
```

모든 핸들러의 패턴: **가드 → (파괴적이면 확인) → 선택 루프 → 항목별 `try/catch` → `FileService` 호출 → `LoadFiles()` 로 갱신.** 주목할 변주:

- **이름 변경**은 정확히 1개 선택을 요구(`Count != 1`)하고, `InputDialog.Show` 에 현재 이름을 미리 채우며, 빈 입력은 `string.IsNullOrWhiteSpace` 로 걸러 종료합니다.
- **복사**와 **이동**은 `using` 블록 안에서 **`FolderBrowserDialog`** 를 열고(대화상자 자동 dispose), `dlg.ShowDialog() == DialogResult.OK` 일 때만 진행합니다. 복사는 목록을 갱신하지 *않고*(원본 폴더가 그대로니까), 이동은 갱신합니다.

### 6.5 자동 정리 핸들러 — `btnAutoOrganize_Click`

계획 → 미리보기 → 결과 → 되돌리기 상태를 엮는 지휘자입니다.

```csharp
List<OrganizePlanItem> plan = AutoOrganizeService.BuildPlan(_allFiles, _currentFolder);
if (plan.Count == 0) { MessageBox.Show("정리할 파일이 없습니다. ..."); return; }   // 할 일 없음

using (AutoOrganizeForm dlg = new AutoOrganizeForm(plan))   // 미리보기 창, 자동 dispose
{
    DialogResult dr = dlg.ShowDialog(this);                 // 모달; 닫힐 때까지 블록
    if (dr != DialogResult.OK || dlg.Result == null)        // 사용자가 취소
    { SetStatus("자동 정리 취소됨"); return; }

    LoadFiles();                                            // 파일 이동됨 → 갱신
    SetStatus("자동 정리 완료: 성공 " + dlg.Result.SuccessCount + " ...");

    if (dlg.Result.SuccessCount > 0)
    {
        _lastOrganizeResult = dlg.Result;   // ← 되돌리기용 영수증 보관
        btnUndo.Enabled = true;             //   되돌리기 버튼 활성화
    }
    HideNotification();                     // 방금 정리함 → 배너 숨김
}
```

- 계획을 `AutoOrganizeForm` 에 넘기고, 확인 + `Execute` 는 그 창이 처리하리라 믿습니다. 계약은 `DialogResult.OK` + null이 아닌 `Result` 입니다.
- **전체가 `using` 으로 감싸여**, 예외가 나도 대화상자 자원이 해제됩니다. 바깥 메서드 자체도 `try/catch` 로 감싸 예기치 못한 오류를 `MessageBox` 로 알립니다.
- `dlg.Result` 를 `_lastOrganizeResult` 에 저장하고 `btnUndo` 를 활성화하는 것이 되돌리기 기능을 장전합니다.

### 6.6 되돌리기 핸들러 — `btnUndo_Click`

```csharp
if (_lastOrganizeResult == null || _lastOrganizeResult.ExecutedMoves.Count == 0)
{ MessageBox.Show("되돌릴 작업이 없습니다."); return; }

int moveCount = _lastOrganizeResult.ExecutedMoves.Count;
if (MessageBox.Show("... " + moveCount + "개 파일을 원래 위치로 되돌립니다. 계속?",
        "되돌리기", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
    return;

this.Cursor = Cursors.WaitCursor;
AutoOrganizeResult undoResult = AutoOrganizeService.Undo(_lastOrganizeResult);  // 실행
this.Cursor = Cursors.Default;

_lastOrganizeResult = null;   // 두 번 되돌릴 수 없게
btnUndo.Enabled = false;
MessageBox.Show("... 복원 " + undoResult.SuccessCount + " / 실패 / 건너뜀 ...");
LoadFiles();
```

확인 후 실제 작업은 `AutoOrganizeService.Undo` 에 위임하고, 그다음 **`_lastOrganizeResult` 를 비우고 버튼을 비활성화**해 같은 배치를 두 번 되돌리지 못하게 합니다. 모래시계 커서가 느릴 수 있는 작업을 감쌉니다.

### 6.7 두 타이머 — *살펴보고 알리되, 절대 옮기지 않음*

**`RefreshTimer_Tick`(5초마다) — 폴더가 실제로 바뀌었을 때만, 그리고 사용자를 방해하지 않을 때만 목록 자동 갱신:**

```csharp
if (string.IsNullOrEmpty(_currentFolder) || !Directory.Exists(_currentFolder)) return;

DateTime nowWriteTime = new DirectoryInfo(_currentFolder).LastWriteTime;
if (nowWriteTime == _lastFolderWriteTime) return;    // 변화 없음 → 건너뜀 (CPU/IO 절약)

// "사용자 방해 금지" 가드:
if (listViewFiles.SelectedItems.Count > 0) return;   //   선택이 사라지면 곤란
if (txtSearch.Focused) return;                       //   검색어 입력 중
if (contextMenu != null && contextMenu.Visible) return; // 메뉴가 열려 있음

LoadFiles();                                         // 안전 → 목록만 갱신
```

- **`LastWriteTime` 으로 변경 감지** — 마지막 로드 이후 폴더 내용이 바뀌었을 때만 실제 작업을 합니다. 아니면 즉시 반환하죠.
- 세 가지 **가드 조건**은 세심한 UX입니다. 갱신은 `ListView` 를 다시 만들어 선택을 지우거나, 타이핑을 방해하거나, 컨텍스트 메뉴를 닫아버립니다. 그래서 그런 상황이면 갱신을 미룹니다.
- 본체 전체가 **빈 catch** 의 `try/catch` 안에 있습니다 — 백그라운드 틱이 사용자에게 오류 대화상자를 띄워서는 절대 안 되니까요.

**`ScanTimer_Tick`(30초마다) — 정리 가능 파일을 감지해 배너 표시(이동 없음):**

```csharp
List<FileInfo> currentFiles = FileService.LoadFiles(_currentFolder);   // 디스크에서 새로 읽음
List<OrganizePlanItem> plan = AutoOrganizeService.BuildPlan(currentFiles, _currentFolder);
int targetCount = plan.Count;

if (targetCount == 0) { HideNotification(); _lastNotifiedCount = -1; return; }   // 할 일 없음
if (targetCount == _lastNotifiedCount && _isNotificationVisible) return;         // 이미 표시 중
if (!this.CanFocus) return;                  // 대화상자가 떠 있음 → 알리지 않음

ShowNotification(targetCount);
_lastNotifiedCount = targetCount;
```

- **`BuildPlan` 을 순전히 카운터로 재사용**합니다 — 계획 생성에는 부작용이 없으므로 "정리 가능한 파일이 몇 개?"는 그냥 `plan.Count` 입니다. 예행연습의 멋진 재사용이죠.
- **`_lastNotifiedCount`** 가 중복을 제거합니다. 배너가 이미 "5개"라고 떠 있고 여전히 5개면 재발화하지 않습니다. 개수가 바뀌면(새 다운로드 도착) 다시 알립니다.
- **`!this.CanFocus`** 는 모달 대화상자(예: 미리보기)가 열려 있는 동안 배너를 억제합니다.

배너 자체는 표시/숨김 헬퍼일 뿐인데, 결정적으로 그 "정리" 버튼이 직접 파일을 옮기지 않고 **일반 확인 흐름으로 되돌아갑니다.**

```csharp
private void btnNotifyOrganize_Click(object sender, EventArgs e)
{
    HideNotification();
    btnAutoOrganize_Click(sender, e);   // → 미리보기 대화상자 → 사용자 확인 → Execute
}
```

알림/이동 분리가 가장 명확히 드러나는 지점입니다. 알림의 액션 버튼조차 동일한 미리보기-확인 경로를 거치며, 사용자 모르게 파일이 옮겨지는 일은 절대 없습니다.

---

## 7. 반복되는 패턴 — 정리

몇 가지 패턴만 체득하면 코드 전체가 쉽게 읽힙니다.

- **계층별 책임 분리.** UI는 `System.IO` 를 직접 건드리지 않고 `FileService` / `AutoOrganizeService` 를 호출합니다. 헬퍼는 순수 규칙을 갖습니다. 그래서 같은 로직(분류, 포맷)이 한 번만 정의되고 어디서나 재사용됩니다.
- **계획 → 확인 → 실행 → 기록.** 정리 기능은 미리볼 수 있는 계획을 먼저 세우고 명시적 확인을 받기 전에는 디스크에 손대지 않으며, 각 작업을 기록해 되돌릴 수 있게 합니다.
- **멱등성과 안전성.** 정리를 다시 돌려도 이미 분류된 파일은 건너뛰고, 충돌은 덮어쓰기 대신 ` (1)` 접미사를, 되돌리기는 ` (복원 N)` 접미사를 붙입니다. 연산이 반복해도 안전하도록 설계되었습니다.
- **항목별 오류 격리.** 배치 작업은 각 항목을 `try/catch` 로 감싸 중단 대신 실패를 집계합니다 — 잠긴 파일 하나가 나머지를 망치지 않습니다.
- **곳곳의 가드 절.** 메서드는 맨 위에서 null/빈 값/없음을 검사해 안전한 기본값을 반환하므로, 정상 경로가 들여쓰기 없이 명료하게 유지됩니다.
- **UI 행의 `Tag` 기반 정체성.** `"DIR:"` 접두어가 붙은 `Tag` 덕분에 하나의 `ListView` 가 폴더와 파일을 함께 담고 클릭 시 올바르게 분기합니다.
- **알림 vs 행동.** 타이머와 배너는 UI 스레드에서 관찰하고 알리기만 합니다. 파일 이동은 오직 사용자가 확인한 경로로만 일어납니다.
- **데이터 가공은 LINQ, 실제 작업은 `System.IO`.** `Where`/`OrderBy`/`GroupBy`/`Distinct`/`Any` 가 필터링과 그룹화를 선언적으로 표현하고, `File.Move`/`Directory.CreateDirectory` 등이 실제 효과를 수행합니다.
