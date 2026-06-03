using System.Drawing;

namespace Download_file_manager
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnCut = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnShare = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAutoOrganize = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.panelNotification = new System.Windows.Forms.Panel();
            this.lblNotification = new System.Windows.Forms.Label();
            this.btnNotifyOrganize = new System.Windows.Forms.Button();
            this.btnNotifyClose = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMove = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.panelPreview = new System.Windows.Forms.Panel();
            this.lblPreviewPath = new System.Windows.Forms.Label();
            this.lblPreviewDate = new System.Windows.Forms.Label();
            this.lblPreviewSize = new System.Windows.Forms.Label();
            this.lblPreviewName = new System.Windows.Forms.Label();
            this.lblPreviewInfo = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.lblPreviewTitle = new System.Windows.Forms.Label();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnGallery = new System.Windows.Forms.Button();
            this.btnDesktop = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnDocument = new System.Windows.Forms.Button();
            this.btnPicture = new System.Windows.Forms.Button();
            this.btnMusic = new System.Windows.Forms.Button();
            this.btnVideo = new System.Windows.Forms.Button();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.lblTotalSize = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnOpen = new System.Windows.Forms.Button();
            this.cmbFilter = new System.Windows.Forms.ComboBox();
            this.btnSortOrder = new System.Windows.Forms.Button();
            this.cmbSort = new System.Windows.Forms.ComboBox();
            this.panelTop.SuspendLayout();
            this.panelCommand.SuspendLayout();
            this.panelNotification.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelSidebar.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.btnBack);
            this.panelTop.Controls.Add(this.btnForward);
            this.panelTop.Controls.Add(this.btnUp);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.lblAddress);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(17, 12, 17, 10);
            this.panelTop.Size = new System.Drawing.Size(1800, 70);
            this.panelTop.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnBack.Location = new System.Drawing.Point(17, 14);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(54, 41);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "‹";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnForward
            // 
            this.btnForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnForward.FlatAppearance.BorderSize = 0;
            this.btnForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForward.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.btnForward.ForeColor = System.Drawing.Color.Gray;
            this.btnForward.Location = new System.Drawing.Point(77, 14);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(54, 41);
            this.btnForward.TabIndex = 1;
            this.btnForward.Text = "›";
            this.btnForward.UseVisualStyleBackColor = false;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.btnUp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnUp.Location = new System.Drawing.Point(140, 14);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(54, 41);
            this.btnUp.TabIndex = 2;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnRefresh.Location = new System.Drawing.Point(203, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(54, 41);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "↻";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblAddress
            // 
            this.lblAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblAddress.Location = new System.Drawing.Point(279, 14);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(980, 41);
            this.lblAddress.TabIndex = 4;
            this.lblAddress.Text = "  🖥  >  다운로드";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.ForeColor = System.Drawing.Color.Gray;
            this.txtSearch.Location = new System.Drawing.Point(1280, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(480, 34);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.Text = " ";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // panelCommand
            // 
            this.panelCommand.BackColor = System.Drawing.Color.White;
            this.panelCommand.Controls.Add(this.btnNew);
            this.panelCommand.Controls.Add(this.btnCut);
            this.panelCommand.Controls.Add(this.btnCopy);
            this.panelCommand.Controls.Add(this.btnPaste);
            this.panelCommand.Controls.Add(this.btnRename);
            this.panelCommand.Controls.Add(this.btnShare);
            this.panelCommand.Controls.Add(this.btnDelete);
            this.panelCommand.Controls.Add(this.btnAutoOrganize);
            this.panelCommand.Controls.Add(this.btnUndo);
            this.panelCommand.Controls.Add(this.cmbSort);
            this.panelCommand.Controls.Add(this.btnSortOrder);
            this.panelCommand.Controls.Add(this.cmbFilter);
            this.panelCommand.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCommand.Location = new System.Drawing.Point(0, 70);
            this.panelCommand.Name = "panelCommand";
            this.panelCommand.Padding = new System.Windows.Forms.Padding(17, 11, 17, 10);
            this.panelCommand.Size = new System.Drawing.Size(1800, 61);
            this.panelCommand.TabIndex = 1;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnNew.Location = new System.Drawing.Point(17, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(179, 41);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "＋ 새로 만들기";
            this.btnNew.UseVisualStyleBackColor = false;
            // 
            // btnCut
            // 
            this.btnCut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnCut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCut.FlatAppearance.BorderSize = 0;
            this.btnCut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnCut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCut.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnCut.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnCut.Location = new System.Drawing.Point(214, 12);
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(66, 41);
            this.btnCut.TabIndex = 1;
            this.btnCut.Text = "✂";
            this.btnCut.UseVisualStyleBackColor = false;
            // 
            // btnCopy
            // 
            this.btnCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCopy.FlatAppearance.BorderSize = 0;
            this.btnCopy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnCopy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnCopy.Location = new System.Drawing.Point(291, 12);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(66, 41);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "⧉";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnPaste.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPaste.FlatAppearance.BorderSize = 0;
            this.btnPaste.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnPaste.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPaste.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnPaste.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnPaste.Location = new System.Drawing.Point(369, 12);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(66, 41);
            this.btnPaste.TabIndex = 3;
            this.btnPaste.Text = "▣";
            this.btnPaste.UseVisualStyleBackColor = false;
            // 
            // btnRename
            // 
            this.btnRename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnRename.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRename.FlatAppearance.BorderSize = 0;
            this.btnRename.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRename.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnRename.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnRename.Location = new System.Drawing.Point(446, 12);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(66, 41);
            this.btnRename.TabIndex = 4;
            this.btnRename.Text = "A";
            this.btnRename.UseVisualStyleBackColor = false;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnShare
            // 
            this.btnShare.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnShare.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShare.FlatAppearance.BorderSize = 0;
            this.btnShare.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnShare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShare.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnShare.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnShare.Location = new System.Drawing.Point(523, 12);
            this.btnShare.Name = "btnShare";
            this.btnShare.Size = new System.Drawing.Size(66, 41);
            this.btnShare.TabIndex = 5;
            this.btnShare.Text = "↗";
            this.btnShare.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnDelete.Location = new System.Drawing.Point(600, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(66, 41);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "🗑";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAutoOrganize
            // 
            this.btnAutoOrganize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnAutoOrganize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAutoOrganize.FlatAppearance.BorderSize = 0;
            this.btnAutoOrganize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnAutoOrganize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAutoOrganize.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnAutoOrganize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnAutoOrganize.Location = new System.Drawing.Point(678, 12);
            this.btnAutoOrganize.Name = "btnAutoOrganize";
            this.btnAutoOrganize.Size = new System.Drawing.Size(132, 41);
            this.btnAutoOrganize.TabIndex = 10;
            this.btnAutoOrganize.Text = "✨ 자동 정리";
            this.btnAutoOrganize.UseVisualStyleBackColor = false;
            this.btnAutoOrganize.Click += new System.EventHandler(this.btnAutoOrganize_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnUndo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUndo.Enabled = false;
            this.btnUndo.FlatAppearance.BorderSize = 0;
            this.btnUndo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUndo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnUndo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnUndo.Location = new System.Drawing.Point(820, 12);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(112, 41);
            this.btnUndo.TabIndex = 11;
            this.btnUndo.Text = "↶ 되돌리기";
            this.btnUndo.UseVisualStyleBackColor = false;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.listViewFiles);
            this.panelMain.Controls.Add(this.panelPreview);
            this.panelMain.Controls.Add(this.panelSidebar);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 131);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1800, 697);
            this.panelMain.TabIndex = 2;
            // 
            // listViewFiles
            // 
            this.listViewFiles.BackColor = System.Drawing.Color.White;
            this.listViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSize,
            this.colType,
            this.colDate});
            this.listViewFiles.ContextMenuStrip = this.contextMenu;
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFiles.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.listViewFiles.ForeColor = System.Drawing.Color.Black;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new System.Drawing.Point(343, 0);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(986, 697);
            this.listViewFiles.SmallImageList = this.imageList;
            this.listViewFiles.TabIndex = 0;
            this.listViewFiles.TileSize = new System.Drawing.Size(420, 48);
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Tile;
            this.listViewFiles.SelectedIndexChanged += new System.EventHandler(this.listViewFiles_SelectedIndexChanged);
            this.listViewFiles.DoubleClick += new System.EventHandler(this.listViewFiles_DoubleClick);
            // 
            // colName
            // 
            this.colName.Text = "이름";
            this.colName.Width = 360;
            // 
            // colSize
            // 
            this.colSize.Text = "크기";
            this.colSize.Width = 120;
            // 
            // colType
            // 
            this.colType.Text = "종류";
            this.colType.Width = 150;
            // 
            // colDate
            // 
            this.colDate.Text = "수정한 날짜";
            this.colDate.Width = 190;
            // 
            // contextMenu
            // 
            this.contextMenu.BackColor = System.Drawing.Color.White;
            this.contextMenu.ForeColor = System.Drawing.Color.Black;
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpen,
            this.menuRename,
            this.menuCopy,
            this.menuMove,
            this.menuSep1,
            this.menuDelete,
            this.menuSep2,
            this.menuProperties});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(163, 208);
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.Size = new System.Drawing.Size(162, 32);
            this.menuOpen.Text = "열기";
            this.menuOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // menuRename
            // 
            this.menuRename.Name = "menuRename";
            this.menuRename.Size = new System.Drawing.Size(162, 32);
            this.menuRename.Text = "이름 변경";
            this.menuRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.Size = new System.Drawing.Size(162, 32);
            this.menuCopy.Text = "복사";
            this.menuCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // menuMove
            // 
            this.menuMove.Name = "menuMove";
            this.menuMove.Size = new System.Drawing.Size(162, 32);
            this.menuMove.Text = "이동";
            this.menuMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // menuSep1
            // 
            this.menuSep1.Name = "menuSep1";
            this.menuSep1.Size = new System.Drawing.Size(159, 6);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(162, 32);
            this.menuDelete.Text = "삭제";
            this.menuDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // menuSep2
            // 
            this.menuSep2.Name = "menuSep2";
            this.menuSep2.Size = new System.Drawing.Size(159, 6);
            // 
            // menuProperties
            // 
            this.menuProperties.Name = "menuProperties";
            this.menuProperties.Size = new System.Drawing.Size(162, 32);
            this.menuProperties.Text = "속성";
            this.menuProperties.Click += new System.EventHandler(this.menuProperties_Click);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(20, 20);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panelPreview
            // 
            this.panelPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.panelPreview.Controls.Add(this.lblPreviewPath);
            this.panelPreview.Controls.Add(this.lblPreviewDate);
            this.panelPreview.Controls.Add(this.lblPreviewSize);
            this.panelPreview.Controls.Add(this.lblPreviewName);
            this.panelPreview.Controls.Add(this.lblPreviewInfo);
            this.panelPreview.Controls.Add(this.pictureBoxPreview);
            this.panelPreview.Controls.Add(this.lblPreviewTitle);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPreview.Location = new System.Drawing.Point(1329, 0);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Padding = new System.Windows.Forms.Padding(29, 24, 29, 24);
            this.panelPreview.Size = new System.Drawing.Size(471, 697);
            this.panelPreview.TabIndex = 1;
            this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPreview_Paint);
            // 
            // lblPreviewPath
            // 
            this.lblPreviewPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewPath.ForeColor = System.Drawing.Color.Gray;
            this.lblPreviewPath.Location = new System.Drawing.Point(29, 473);
            this.lblPreviewPath.Name = "lblPreviewPath";
            this.lblPreviewPath.Size = new System.Drawing.Size(413, 108);
            this.lblPreviewPath.TabIndex = 0;
            this.lblPreviewPath.Text = "경로: -";
            // 
            // lblPreviewDate
            // 
            this.lblPreviewDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewDate.ForeColor = System.Drawing.Color.Gray;
            this.lblPreviewDate.Location = new System.Drawing.Point(29, 432);
            this.lblPreviewDate.Name = "lblPreviewDate";
            this.lblPreviewDate.Size = new System.Drawing.Size(413, 41);
            this.lblPreviewDate.TabIndex = 1;
            this.lblPreviewDate.Text = "날짜: -";
            // 
            // lblPreviewSize
            // 
            this.lblPreviewSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewSize.ForeColor = System.Drawing.Color.Gray;
            this.lblPreviewSize.Location = new System.Drawing.Point(29, 391);
            this.lblPreviewSize.Name = "lblPreviewSize";
            this.lblPreviewSize.Size = new System.Drawing.Size(413, 41);
            this.lblPreviewSize.TabIndex = 2;
            this.lblPreviewSize.Text = "크기: -";
            // 
            // lblPreviewName
            // 
            this.lblPreviewName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewName.ForeColor = System.Drawing.Color.Gray;
            this.lblPreviewName.Location = new System.Drawing.Point(29, 350);
            this.lblPreviewName.Name = "lblPreviewName";
            this.lblPreviewName.Size = new System.Drawing.Size(413, 41);
            this.lblPreviewName.TabIndex = 3;
            this.lblPreviewName.Text = "이름: -";
            // 
            // lblPreviewInfo
            // 
            this.lblPreviewInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPreviewInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblPreviewInfo.Location = new System.Drawing.Point(29, 300);
            this.lblPreviewInfo.Name = "lblPreviewInfo";
            this.lblPreviewInfo.Size = new System.Drawing.Size(413, 50);
            this.lblPreviewInfo.TabIndex = 4;
            this.lblPreviewInfo.Text = "파일 정보";
            this.lblPreviewInfo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.White;
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxPreview.Location = new System.Drawing.Point(29, 84);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(413, 216);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 5;
            this.pictureBoxPreview.TabStop = false;
            // 
            // lblPreviewTitle
            // 
            this.lblPreviewTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblPreviewTitle.Location = new System.Drawing.Point(29, 24);
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.Size = new System.Drawing.Size(413, 60);
            this.lblPreviewTitle.TabIndex = 6;
            this.lblPreviewTitle.Text = "다운로드";
            this.lblPreviewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.panelSidebar.Controls.Add(this.btnHome);
            this.panelSidebar.Controls.Add(this.btnGallery);
            this.panelSidebar.Controls.Add(this.btnDesktop);
            this.panelSidebar.Controls.Add(this.btnDownload);
            this.panelSidebar.Controls.Add(this.btnDocument);
            this.panelSidebar.Controls.Add(this.btnPicture);
            this.panelSidebar.Controls.Add(this.btnMusic);
            this.panelSidebar.Controls.Add(this.btnVideo);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Padding = new System.Windows.Forms.Padding(14, 24, 14, 12);
            this.panelSidebar.Size = new System.Drawing.Size(343, 697);
            this.panelSidebar.TabIndex = 2;
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnHome.Location = new System.Drawing.Point(17, 24);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(300, 50);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "🏠  홈";
            this.btnHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHome.UseVisualStyleBackColor = false;
            // 
            // btnGallery
            // 
            this.btnGallery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnGallery.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGallery.FlatAppearance.BorderSize = 0;
            this.btnGallery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnGallery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGallery.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGallery.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnGallery.Location = new System.Drawing.Point(17, 82);
            this.btnGallery.Name = "btnGallery";
            this.btnGallery.Size = new System.Drawing.Size(300, 50);
            this.btnGallery.TabIndex = 1;
            this.btnGallery.Text = "🖼  갤러리";
            this.btnGallery.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGallery.UseVisualStyleBackColor = false;
            // 
            // btnDesktop
            // 
            this.btnDesktop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnDesktop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDesktop.FlatAppearance.BorderSize = 0;
            this.btnDesktop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnDesktop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesktop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDesktop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnDesktop.Location = new System.Drawing.Point(17, 139);
            this.btnDesktop.Name = "btnDesktop";
            this.btnDesktop.Size = new System.Drawing.Size(300, 50);
            this.btnDesktop.TabIndex = 2;
            this.btnDesktop.Text = "🖥  바탕 화면";
            this.btnDesktop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDesktop.UseVisualStyleBackColor = false;
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.btnDownload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownload.FlatAppearance.BorderSize = 0;
            this.btnDownload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownload.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDownload.ForeColor = System.Drawing.Color.DodgerBlue;
            this.btnDownload.Location = new System.Drawing.Point(17, 197);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(300, 50);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "↓  다운로드";
            this.btnDownload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.UseVisualStyleBackColor = false;
            // 
            // btnDocument
            // 
            this.btnDocument.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnDocument.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDocument.FlatAppearance.BorderSize = 0;
            this.btnDocument.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnDocument.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDocument.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDocument.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnDocument.Location = new System.Drawing.Point(17, 254);
            this.btnDocument.Name = "btnDocument";
            this.btnDocument.Size = new System.Drawing.Size(300, 50);
            this.btnDocument.TabIndex = 4;
            this.btnDocument.Text = "📄  문서";
            this.btnDocument.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDocument.UseVisualStyleBackColor = false;
            // 
            // btnPicture
            // 
            this.btnPicture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnPicture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPicture.FlatAppearance.BorderSize = 0;
            this.btnPicture.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnPicture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPicture.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnPicture.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnPicture.Location = new System.Drawing.Point(17, 312);
            this.btnPicture.Name = "btnPicture";
            this.btnPicture.Size = new System.Drawing.Size(300, 50);
            this.btnPicture.TabIndex = 5;
            this.btnPicture.Text = "🖼  사진";
            this.btnPicture.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPicture.UseVisualStyleBackColor = false;
            // 
            // btnMusic
            // 
            this.btnMusic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnMusic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMusic.FlatAppearance.BorderSize = 0;
            this.btnMusic.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnMusic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMusic.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnMusic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnMusic.Location = new System.Drawing.Point(17, 370);
            this.btnMusic.Name = "btnMusic";
            this.btnMusic.Size = new System.Drawing.Size(300, 50);
            this.btnMusic.TabIndex = 6;
            this.btnMusic.Text = "🎵  음악";
            this.btnMusic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMusic.UseVisualStyleBackColor = false;
            // 
            // btnVideo
            // 
            this.btnVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnVideo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVideo.FlatAppearance.BorderSize = 0;
            this.btnVideo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVideo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnVideo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnVideo.Location = new System.Drawing.Point(17, 427);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(300, 50);
            this.btnVideo.TabIndex = 7;
            this.btnVideo.Text = "🎬  동영상";
            this.btnVideo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVideo.UseVisualStyleBackColor = false;
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.panelStatus.Controls.Add(this.lblStatus);
            this.panelStatus.Controls.Add(this.lblFileCount);
            this.panelStatus.Controls.Add(this.lblTotalSize);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 828);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(1800, 36);
            this.panelStatus.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(17, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(44, 18);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "준비";
            // 
            // lblFileCount
            // 
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.ForeColor = System.Drawing.Color.Gray;
            this.lblFileCount.Location = new System.Drawing.Point(400, 8);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(78, 18);
            this.lblFileCount.TabIndex = 1;
            this.lblFileCount.Text = "0개 파일";
            // 
            // lblTotalSize
            // 
            this.lblTotalSize.AutoSize = true;
            this.lblTotalSize.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalSize.Location = new System.Drawing.Point(614, 8);
            this.lblTotalSize.Name = "lblTotalSize";
            this.lblTotalSize.Size = new System.Drawing.Size(122, 18);
            this.lblTotalSize.TabIndex = 2;
            this.lblTotalSize.Text = "총 크기: 0 MB";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(0, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Visible = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // cmbFilter
            // 
            this.cmbFilter.BackColor = System.Drawing.Color.White;
            this.cmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter.ForeColor = System.Drawing.Color.Black;
            this.cmbFilter.Items.AddRange(new object[] {
            "전체",
            "이미지",
            "동영상",
            "문서",
            "압축파일",
            "실행파일",
            "기타"});
            this.cmbFilter.Location = new System.Drawing.Point(1036, 14);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(184, 26);
            this.cmbFilter.TabIndex = 9;
            this.cmbFilter.Visible = false;
            this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
            // 
            // btnSortOrder
            // 
            this.btnSortOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.btnSortOrder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSortOrder.FlatAppearance.BorderSize = 0;
            this.btnSortOrder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnSortOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSortOrder.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnSortOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnSortOrder.Location = new System.Drawing.Point(900, 12);
            this.btnSortOrder.Name = "btnSortOrder";
            this.btnSortOrder.Size = new System.Drawing.Size(114, 41);
            this.btnSortOrder.TabIndex = 8;
            this.btnSortOrder.Text = "▲ 오름";
            this.btnSortOrder.UseVisualStyleBackColor = false;
            this.btnSortOrder.Visible = false;
            this.btnSortOrder.Click += new System.EventHandler(this.btnSortOrder_Click);
            // 
            // cmbSort
            // 
            this.cmbSort.BackColor = System.Drawing.Color.White;
            this.cmbSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSort.ForeColor = System.Drawing.Color.Black;
            this.cmbSort.Items.AddRange(new object[] {
            "이름",
            "크기",
            "종류",
            "날짜"});
            this.cmbSort.Location = new System.Drawing.Point(710, 22);
            this.cmbSort.Name = "cmbSort";
            this.cmbSort.Size = new System.Drawing.Size(170, 26);
            this.cmbSort.TabIndex = 7;
            this.cmbSort.Visible = false;
            this.cmbSort.SelectedIndexChanged += new System.EventHandler(this.cmbSort_SelectedIndexChanged);
            // 
            // panelNotification (개선 3: 알림 배너)
            // 
            this.panelNotification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(225)))));
            this.panelNotification.Controls.Add(this.lblNotification);
            this.panelNotification.Controls.Add(this.btnNotifyOrganize);
            this.panelNotification.Controls.Add(this.btnNotifyClose);
            this.panelNotification.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelNotification.Location = new System.Drawing.Point(0, 131);
            this.panelNotification.Name = "panelNotification";
            this.panelNotification.Size = new System.Drawing.Size(1800, 44);
            this.panelNotification.TabIndex = 0;
            this.panelNotification.Visible = false;
            // 
            // lblNotification
            // 
            this.lblNotification.AutoSize = false;
            this.lblNotification.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblNotification.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(80)))), ((int)(((byte)(0)))));
            this.lblNotification.Location = new System.Drawing.Point(20, 0);
            this.lblNotification.Name = "lblNotification";
            this.lblNotification.Size = new System.Drawing.Size(1500, 44);
            this.lblNotification.TabIndex = 0;
            this.lblNotification.Text = "🔔 새로 추가된 정리 가능 파일이 있습니다.";
            this.lblNotification.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNotifyOrganize
            // 
            this.btnNotifyOrganize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNotifyOrganize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnNotifyOrganize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotifyOrganize.FlatAppearance.BorderSize = 0;
            this.btnNotifyOrganize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotifyOrganize.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnNotifyOrganize.ForeColor = System.Drawing.Color.White;
            this.btnNotifyOrganize.Location = new System.Drawing.Point(1640, 7);
            this.btnNotifyOrganize.Name = "btnNotifyOrganize";
            this.btnNotifyOrganize.Size = new System.Drawing.Size(110, 30);
            this.btnNotifyOrganize.TabIndex = 0;
            this.btnNotifyOrganize.Text = "지금 정리";
            this.btnNotifyOrganize.UseVisualStyleBackColor = false;
            this.btnNotifyOrganize.Click += new System.EventHandler(this.btnNotifyOrganize_Click);
            // 
            // btnNotifyClose
            // 
            this.btnNotifyClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNotifyClose.BackColor = System.Drawing.Color.Transparent;
            this.btnNotifyClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotifyClose.FlatAppearance.BorderSize = 0;
            this.btnNotifyClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotifyClose.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnNotifyClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnNotifyClose.Location = new System.Drawing.Point(1758, 7);
            this.btnNotifyClose.Name = "btnNotifyClose";
            this.btnNotifyClose.Size = new System.Drawing.Size(30, 30);
            this.btnNotifyClose.TabIndex = 0;
            this.btnNotifyClose.Text = "✕";
            this.btnNotifyClose.UseVisualStyleBackColor = false;
            this.btnNotifyClose.Click += new System.EventHandler(this.btnNotifyClose_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1800, 864);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panelNotification);
            this.Controls.Add(this.panelCommand);
            this.Controls.Add(this.panelTop);
            this.MinimumSize = new System.Drawing.Size(1419, 709);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "다운로드";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelCommand.ResumeLayout(false);
            this.panelNotification.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelSidebar.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.Panel panelCommand;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnCut;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAutoOrganize;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Panel panelNotification;
        private System.Windows.Forms.Label lblNotification;
        private System.Windows.Forms.Button btnNotifyOrganize;
        private System.Windows.Forms.Button btnNotifyClose;

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnGallery;
        private System.Windows.Forms.Button btnDesktop;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnDocument;
        private System.Windows.Forms.Button btnPicture;
        private System.Windows.Forms.Button btnMusic;
        private System.Windows.Forms.Button btnVideo;

        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colDate;

        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label lblPreviewTitle;
        private System.Windows.Forms.Label lblPreviewInfo;
        private System.Windows.Forms.Label lblPreviewName;
        private System.Windows.Forms.Label lblPreviewSize;
        private System.Windows.Forms.Label lblPreviewDate;
        private System.Windows.Forms.Label lblPreviewPath;

        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Label lblTotalSize;

        private System.Windows.Forms.Button btnOpen;

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuRename;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuMove;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripSeparator menuSep2;
        private System.Windows.Forms.ToolStripMenuItem menuProperties;

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ComboBox cmbSort;
        private System.Windows.Forms.Button btnSortOrder;
        private System.Windows.Forms.ComboBox cmbFilter;
    }
}