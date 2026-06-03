// ==========================================
// 사용자 입력창(Input Dialog)을 생성하는 클래스
//
// 주요 기능:
// - 이름 변경 입력창 표시
// - 사용자 문자열 입력 반환
// ==========================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Form, Label, TextBox, Button 사용

namespace Download_file_manager // 프로젝트 namespace
{
    // 간단한 입력창을 직접 만들어주는 클래스
    public static class InputDialog
    {
        // 제목, 안내문, 기본값을 받아 입력창을 띄우고 입력값을 반환하는 함수
        public static string Show(string title, string prompt, string defaultValue)
        {
            // 입력창으로 사용할 Form 생성
            Form form = new Form();

            // 창 제목 설정
            form.Text = title;

            // 창 너비 설정
            form.Width = 360;

            // 창 높이 설정
            form.Height = 150;

            // 부모 창 가운데에 뜨도록 설정
            form.StartPosition = FormStartPosition.CenterParent;

            // 창 크기 고정
            form.FormBorderStyle = FormBorderStyle.FixedDialog;

            // 최대화 버튼 비활성화
            form.MaximizeBox = false;

            // 최소화 버튼 비활성화
            form.MinimizeBox = false;

            // 안내 문구 Label 생성
            Label lbl = new Label();

            // Label에 표시할 문구 설정
            lbl.Text = prompt;

            // Label의 왼쪽 위치 설정
            lbl.Left = 12;

            // Label의 위쪽 위치 설정
            lbl.Top = 14;

            // Label 너비 설정
            lbl.Width = 320;

            // 입력용 TextBox 생성
            TextBox txt = new TextBox();

            // TextBox 기본값 설정
            txt.Text = defaultValue;

            // TextBox 왼쪽 위치 설정
            txt.Left = 12;

            // TextBox 위쪽 위치 설정
            txt.Top = 40;

            // TextBox 너비 설정
            txt.Width = 320;

            // 확인 버튼 생성
            Button ok = new Button();

            // 확인 버튼 텍스트 설정
            ok.Text = "확인";

            // 확인 버튼 왼쪽 위치 설정
            ok.Left = 180;

            // 확인 버튼 위쪽 위치 설정
            ok.Top = 72;

            // 확인 버튼 너비 설정
            ok.Width = 75;

            // 확인 버튼을 누르면 OK 결과 반환
            ok.DialogResult = DialogResult.OK;

            // 취소 버튼 생성
            Button cancel = new Button();

            // 취소 버튼 텍스트 설정
            cancel.Text = "취소";

            // 취소 버튼 왼쪽 위치 설정
            cancel.Left = 262;

            // 취소 버튼 위쪽 위치 설정
            cancel.Top = 72;

            // 취소 버튼 너비 설정
            cancel.Width = 75;

            // 취소 버튼을 누르면 Cancel 결과 반환
            cancel.DialogResult = DialogResult.Cancel;

            // Form에 Label 추가
            form.Controls.Add(lbl);

            // Form에 TextBox 추가
            form.Controls.Add(txt);

            // Form에 확인 버튼 추가
            form.Controls.Add(ok);

            // Form에 취소 버튼 추가
            form.Controls.Add(cancel);

            // Enter 키를 누르면 확인 버튼 동작
            form.AcceptButton = ok;

            // ESC 키를 누르면 취소 버튼 동작
            form.CancelButton = cancel;

            // 입력창을 띄우고, 확인을 누르면 입력값 반환, 아니면 null 반환
            return form.ShowDialog() == DialogResult.OK
                ? txt.Text.Trim()
                : null;
        }
    }
}