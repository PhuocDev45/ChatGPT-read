private void StartScan()
        {
            short multiStreamMode = (short)this.cboMultiStreamMode.SelectedIndex;
            short multiStreamFileNameMode = (short)this.cboMultiStreamFileNameMode.SelectedIndex;
            if (multiStreamMode != 0 && multiStreamFileNameMode == 1)
            {
                switch (multiStreamMode)
                {
                    case 1:
                        if (this.txtFileCounterEx1.Text == "-1")
                        {
                            MessageBox.Show("In this sample, \"-1\" cannot be specified for the FileCounterEx1 property.\r\nSpecify an integer greater than or equal to \"0\" or specify \"-2\".", "fiScanTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else if (this.txtFileCounterEx2.Text == "-1")
                        {
                            MessageBox.Show("In this sample, \"-1\" cannot be specified for the FileCounterEx2 property.\r\nSpecify an integer greater than or equal to \"0\" or specify \"-2\".", "fiScanTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        break;
                    case 2:
                        if (this.txtFileCounterEx1.Text == "-1")
                        {
                            MessageBox.Show("In this sample, \"-1\" cannot be specified for the FileCounterEx1 property.\r\nSpecify an integer greater than or equal to \"0\" or specify \"-2\".", "fiScanTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else if (this.txtFileCounterEx2.Text == "-1")
                        {
                            MessageBox.Show("In this sample, \"-1\" cannot be specified for the FileCounterEx2 property.\r\nSpecify an integer greater than or equal to \"0\" or specify \"-2\".", "fiScanTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else if (this.txtFileCounterEx3.Text == "-1")
                        {
                            MessageBox.Show("In this sample, \"-1\" cannot be specified for the FileCounterEx3 property.\r\nSpecify an integer greater than or equal to \"0\" or specify \"-2\".", "fiScanTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (this.txtFileCounterEx.Text == "-1")
                {
                    MessageBox.Show("In this sample, \"-1\" cannot be specified for the FileCounterEx property.\r\nSpecify an integer greater than or equal to \"0\" or specify \"-2\".", "fiScanTest", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            int status;
            int ErrorCode;
            FormCancelScan frmCancelScan = new FormCancelScan();

            //A scanning parameter is set as scanner control.
            ScanModeSet();

            frmCancelScan.StartPosition = FormStartPosition.Manual;
            frmCancelScan.Top = this.Top;
            frmCancelScan.Left = this.Left + this.Width;
            frmCancelScan.Owner = this;
            frmCancelScan.Show();

            //Scanning start
            status = axFiScn1.StartScan(this.Handle.ToInt32());

            //failure
            if(status == ModuleScan.RC_FAILURE)
            {
                ErrorCode = axFiScn1.ErrorCode;
                MessageBox.Show("The scanning error occurred.\n\terror code : " + HexString(ErrorCode), "fiScanTest");
                this.statusBar1.Text = axFiScn1.PageCount + "sheets were scanned.";
                if (ErrorCode == 0x00000002)
                {
                    if (resultList.Count > 0)
                    {
                        FormResultOfDetect frmResultOfDetect = new FormResultOfDetect();
                        frmResultOfDetect.resultArray = (ArrayList)resultList.Clone();
                        frmResultOfDetect.InsertResultOfDetect();
                        frmResultOfDetect.ShowDialog();
                    }
                }
                resultList.Clear();
            }
            else if( status == ModuleScan.RC_CANCEL)
            {
                MessageBox.Show("The user canceled scanning.\nOr the error which cannot continue scanning was detected.", "fiScanTest");
                this.statusBar1.Text = axFiScn1.PageCount + "sheets were scanned.";
                resultList.Clear();
            }
            else
            {
                if (resultList.Count > 0)
                {
                    FormResultOfDetect frmResultOfDetect = new FormResultOfDetect();
                    frmResultOfDetect.resultArray = (ArrayList)resultList.Clone();
                    frmResultOfDetect.InsertResultOfDetect();
                    frmResultOfDetect.ShowDialog();
                    resultList.RemoveRange(0, resultList.Count);
                }
                this.statusBar1.Text = axFiScn1.PageCount + "sheets were scanned.";
            }

            //Update of FileCounterEx
            this.txtFileCounterEx.Text = axFiScn1.FileCounterEx.ToString();
            this.txtFileCounterEx1.Text = axFiScn1.FileCounterEx1.ToString();
            this.txtFileCounterEx2.Text = axFiScn1.FileCounterEx2.ToString();
            this.txtFileCounterEx3.Text = axFiScn1.FileCounterEx3.ToString();

            ModuleScan.multiStreamModeCount = 0;
            ModuleScan.multiStreamModeIndex = 0;

            frmCancelScan.Close();
        }

        //----------------------------------------------------------------------------
        //  Function    : Pretreatment
        //  Argument    : Nothing
        //  Return code : Nothing
        //----------------------------------------------------------------------------
        public void OpenScanner()
        {
            int status;
            int ErrorCode;

            //Call "OpenScanner" method
            status = axFiScn1.OpenScanner(this.Handle.ToInt32());
            if(status == ModuleScan.RC_FAILURE)
            {
                ErrorCode = axFiScn1.ErrorCode;
                MessageBox.Show("The \"OpenScanner\" function became an error.\n\terror code : " + HexString(ErrorCode), "fiScanTest");
                ModuleScan.blnFjtwn = false;
                ModuleScan.blnOpenScanner = false;
            }
            else if(status == ModuleScan.RC_NOT_DS_FJTWAIN)
            {
                MessageBox.Show("It is not \"TWAIN Driver.\"", "fiScanTest");
                ModuleScan.blnFjtwn = false;
            }

            //A "Scan" button is disabled when "OpenScanner" failed.
            if( ModuleScan.blnOpenScanner == false)
            {
                this.ButtonScan.Enabled = false;
                this.MenuItemStartScan.Enabled = false;
                this.MenuItemClearPage.Enabled = false;
            }
            else
            {
                this.ButtonScan.Enabled = true;
                this.MenuItemStartScan.Enabled = true;
                this.MenuItemClearPage.Enabled = true;
            }

            //An image scanner name is displayed on a title.
            if(axFiScn1.ImageScanner.Length > 0)
            {
                this.Text = "Visual C# Sample fiScanTest(" + axFiScn1.ImageScanner + ")";
            }
            else
            {
                this.Text = "Visual C# Sample fiScanTest";
            }
        }

        //----------------------------------------------------------------------------
        //  Function    : The hexadecimal number character string of 8 figures is
        //                generated.
        //  Argument    : ErrorCode
        //  Return code : String
        //----------------------------------------------------------------------------
        public string HexString(int ErrorCode)
        {
            string strWork;
            strWork = ErrorCode.ToString("X");
            if(strWork.Length == 1)
            {
                strWork = "0x0000000" + strWork;
            }
            else if(strWork.Length == 2)
            {
                strWork = "0x000000" + strWork;
            }
            else if(strWork.Length == 3)
            {
                strWork = "0x00000" + strWork;
            }
            else if(strWork.Length == 4)
            {
                strWork = "0x0000" + strWork;
            }
            else if(strWork.Length == 5)
            {
                strWork = "0x000" + strWork;
            }
            else if(strWork.Length == 6)
            {
                strWork = "0x00" + strWork;
            }
            else if(strWork.Length == 7)
            {
                strWork = "0x0" + strWork;
            }
            else 
            {
                strWork = "0x" + strWork;
            }
            return strWork;
        }

        //----------------------------------------------------------------------------
        //  Function    : The parameter information set as scanner control
        //  Argument    : Nothing
        //  Return code : Nothing
        //----------------------------------------------------------------------------
        private void ScanModeSet()
        {
            //Scanner parameter setting
            this.axFiScn1.Outline = ModuleScan.NONE;
            this.axFiScn1.ScanTo = (short)this.cboScanTo.SelectedIndex;
            this.axFiScn1.FileType = (short)this.cboFileType.SelectedIndex;
            this.axFiScn1.FileName = this.txtFileName.Text;
            this.axFiScn1.Overwrite = (short)this.cboOverwrite.SelectedIndex;
            this.axFiScn1.FileCounterEx = System.Int32.Parse(this.txtFileCounterEx.Text);
            this.axFiScn1.CompressionType = (short)this.cboCompType.SelectedIndex;
            this.axFiScn1.JpegQuality = (short)this.cboJpegQuality.SelectedIndex;
            this.axFiScn1.ScanMode = (short)this.cboScanMode.SelectedIndex;
            this.axFiScn1.ScanContinue = this.chkScanContinue.Checked;
            this.axFiScn1.ScanContinueMode = (short)this.cboScanContinueMode.SelectedIndex;
            this.axFiScn1.PixelType = (short)this.cboPixelType.SelectedIndex;
            this.axFiScn1.AutomaticColorSensitivity = short.Parse(this.txtAutomaticColorSensitivity.Text);
            this.axFiScn1.AutomaticColorBackground = (short)this.cboAutomaticColorBackground.SelectedIndex;
            if (this.cboResolution.SelectedIndex == 8)
            {
                this.axFiScn1.Resolution = ModuleScan.RS_1200;
            }
            else if (this.cboResolution.SelectedIndex == 9)
            {
                this.axFiScn1.Resolution = ModuleScan.RS_CUSTM;
            }
            else
            {
                this.axFiScn1.Resolution = (short)this.cboResolution.SelectedIndex;
            }
            this.axFiScn1.CustomResolution = short.Parse(this.txtCustomResolution.Text);
            this.axFiScn1.Brightness = short.Parse(this.txtBrightness.Text);
            this.axFiScn1.Contrast = short.Parse(this.txtContrast.Text);
            this.axFiScn1.Threshold = short.Parse(this.txtThreshold.Text);
            this.axFiScn1.Reverse = this.chkReverse.Checked;
            this.axFiScn1.Mirroring = this.chkMirroring.Checked;
            this.axFiScn1.Rotation = (short)this.cboRotation.SelectedIndex;
            this.axFiScn1.AutomaticRotateMode = (short)(this.cboAutomaticRotateMode.SelectedIndex);
            this.axFiScn1.Background = (short)this.cboBackground.SelectedIndex;
            if(this.cboPixelType.SelectedIndex == ModuleScan.PIXEL_RGB
                && this.cboOutline.SelectedIndex > 3)
            {
                this.axFiScn1.Outline = (short)(this.cboOutline.SelectedIndex + 1);
            }
            else
            {
                this.axFiScn1.Outline = (short)this.cboOutline.SelectedIndex;
            }
            this.axFiScn1.Halftone = (short)this.cboHalftone.SelectedIndex;
            this.axFiScn1.HalftoneFile = this.txtHalftoneFile.Text;
            this.axFiScn1.Gamma = (short)this.cboGamma.SelectedIndex;
            this.axFiScn1.GammaFile = this.txtGammaFile.Text;

            float customGammaValue;
            try
            {
                customGammaValue = float.Parse(txtCustomGamma.Text);
            }
            catch (FormatException)
            {
                customGammaValue = float.Parse(txtCustomGamma.Text.Replace('.', ','));
            }
            this.axFiScn1.CustomGamma = customGammaValue;

            this.axFiScn1.AutoSeparation = (short)this.cboAutoSeparation.SelectedIndex;
            this.axFiScn1.SEE = (short)this.cboSEE.SelectedIndex;
            int iPaperSupply = 0;
            if (this.cboPaperSupply.SelectedIndex >= 8 && this.cboPaperSupply.SelectedIndex <= 47)
            {
                iPaperSupply = 2;
            }
            this.axFiScn1.PaperSupply = (short)(this.cboPaperSupply.SelectedIndex + iPaperSupply);
            this.axFiScn1.ScanCount =short.Parse(this.txtScanCount.Text);
            if(this.cboPaperSize.SelectedIndex == ModuleScan.PSIZE_INDEX_CUSTOM)
            {
                this.axFiScn1.PaperSize = ModuleScan.PSIZE_DATA_CUSTOM;
            }
            else
            {
                this.axFiScn1.PaperSize = (short)this.cboPaperSize.SelectedIndex;
            }
            this.axFiScn1.LongPage = this.chkLongPage.Checked;
            this.axFiScn1.Orientation = (short)this.cboOrientation.SelectedIndex;

            float customPaperWidthValue;
            try
            {
                customPaperWidthValue = float.Parse(txtCustomPaperWidth.Text);
            }
            catch (FormatException)
            {
                customPaperWidthValue = float.Parse(txtCustomPaperWidth.Text.Replace('.', ','));
            }
            this.axFiScn1.CustomPaperWidth = customPaperWidthValue;
            float customPaperLengthValue;
            try
            {
                customPaperLengthValue = float.Parse(txtCustomPaperLength.Text);
            }
            catch (FormatException)
            {
                customPaperLengthValue = float.Parse(txtCustomPaperLength.Text.Replace('.', ','));
            }
            this.axFiScn1.CustomPaperLength = customPaperLengthValue;

            float regionLeftValue;
            try
            {
                regionLeftValue = float.Parse(txtRegionLeft.Text);
            }
            catch (FormatException)
            {
                regionLeftValue = float.Parse(txtRegionLeft.Text.Replace('.', ','));
            }
            this.axFiScn1.RegionLeft = regionLeftValue;

            float regionTopValue;
            try
            {
                regionTopValue = float.Parse(txtRegionTop.Text);
            }
            catch (FormatException)
            {
                regionTopValue = float.Parse(txtRegionTop.Text.Replace('.', ','));
            }
            this.axFiScn1.RegionTop = regionTopValue;

            float regionWidthValue;
            try
            {
                regionWidthValue = float.Parse(txtRegionWidth.Text);
            }
            catch (FormatException)
            {
                regionWidthValue = float.Parse(txtRegionWidth.Text.Replace('.', ','));
            }
            this.axFiScn1.RegionWidth = regionWidthValue;

            float regionLengthValue;
            try
            {
                regionLengthValue = float.Parse(txtRegionLength.Text);
            }
            catch (FormatException)
            {
                regionLengthValue = float.Parse(txtRegionLength.Text.Replace('.', ','));
            }
            this.axFiScn1.RegionLength = regionLengthValue;

            this.axFiScn1.UndefinedScanning = this.chkUndefinedScanning.Checked;
            this.axFiScn1.BackgroundColor = (short)this.cboBackgroundColor.SelectedIndex;
            this.axFiScn1.ThresholdCurve = (short)this.cboThresholdCurve.SelectedIndex;
            this.axFiScn1.NoiseRemoval = (short)this.cboNoiseRemoval.SelectedIndex;
            this.axFiScn1.PreFiltering = this.chkPreFiltering.Checked;
            this.axFiScn1.Smoothing = this.chkSmoothing.Checked;
            this.axFiScn1.Endorser = this.chkEndorser.Checked;
            this.axFiScn1.EndorserDialog = (short)this.cboEndorserDialog.SelectedIndex;

            float endorserOffsetValue;
            try
            {
                endorserOffsetValue = float.Parse(txtEndorserOffset.Text);
            }
            catch (FormatException)
            {
                endorserOffsetValue = float.Parse(txtEndorserOffset.Text.Replace('.', ','));
            }
            this.axFiScn1.EndorserOffset = endorserOffsetValue;

            this.axFiScn1.EndorserString = this.txtEndorserString.Text;
            this.axFiScn1.EndorserCounter = System.Int32.Parse(this.txtEndorserCounter.Text);
            if(this.cboEndorserDirection.SelectedIndex == 1)
            {
                this.axFiScn1.EndorserDirection = 3;
            }
            else
            {
                this.axFiScn1.EndorserDirection = 1;
            }
            this.axFiScn1.EndorserCountStep = (short)this.cboEndorserCountStep.SelectedIndex;
            this.axFiScn1.EndorserCountDirection = (short)this.cboEndorserCountDirection.SelectedIndex;
            this.axFiScn1.EndorserFont = (short)this.cboEndorserFont.SelectedIndex;
            this.axFiScn1.SynchronizationDigitalEndorser = this.chkSynchronizationDigitalEndorser.Checked;
            this.axFiScn1.DigitalEndorser = this.chkDigitalEndorser.Checked;

            float endorserDigitalXOffsetValue;
            try
            {
                endorserDigitalXOffsetValue = float.Parse(txtDigitalEndorserXOffset.Text);
            }
            catch (FormatException)
            {
                endorserDigitalXOffsetValue = float.Parse(txtDigitalEndorserXOffset.Text.Replace('.', ','));
            }
            this.axFiScn1.DigitalEndorserXOffset = endorserDigitalXOffsetValue;
            float endorserDigitalYOffsetValue;
            try
            {
                endorserDigitalYOffsetValue = float.Parse(txtDigitalEndorserYOffset.Text);
            }
            catch (FormatException)
            {
                endorserDigitalYOffsetValue = float.Parse(txtDigitalEndorserYOffset.Text.Replace('.', ','));
            }
            this.axFiScn1.DigitalEndorserYOffset = endorserDigitalYOffsetValue;
            this.axFiScn1.DigitalEndorserString = this.txtDigitalEndorserString.Text;
            this.axFiScn1.DigitalEndorserCounter = System.Int32.Parse(this.txtDigitalEndorserCounter.Text);
            this.axFiScn1.DigitalEndorserDirection = (short)this.cboDigitalEndorserDirection.SelectedIndex;
            this.axFiScn1.DigitalEndorserCountStep = (short)this.cboDigitalEndorserCountStep.SelectedIndex;
            this.axFiScn1.DigitalEndorserCountDirection = (short)this.cboDigitalEndorserCountDirection.SelectedIndex;

            this.axFiScn1.JobControl = (short)this.cboJobControl.SelectedIndex;
            this.axFiScn1.Binding = (short)this.cboBinding.SelectedIndex;
            this.axFiScn1.MultiFeed = (short)this.cboMultiFeed.SelectedIndex;
            if(this.cboFilter.SelectedIndex == 7)
            {
                this.axFiScn1.Filter = 99;
            }
            else if(cboFilter.SelectedIndex == 8)
            {
                this.axFiScn1.Filter = 100;
            }
            else if(this.cboFilter.SelectedIndex == 9)
            {
                this.axFiScn1.Filter = 101;
            }
            else if(this.cboFilter.SelectedIndex == 10)
            {
                this.axFiScn1.Filter = 102;
            }
            else
            {
                this.axFiScn1.Filter = (short)this.cboFilter.SelectedIndex;
            }
            this.axFiScn1.FilterSaturationSensitivity = short.Parse(this.txtFilterSaturationSensitivity.Text);
            this.axFiScn1.SkipWhitePage = short.Parse(this.txtSkipWhitePage.Text);
            this.axFiScn1.SkipBlackPage = short.Parse(this.txtSkipBlackPage.Text);
            this.axFiScn1.AutoBorderDetection = this.chkAutoBorderDetection.Checked;
            this.axFiScn1.ShowSourceUI = this.MenuItemShowSourceUI.Checked;
            this.axFiScn1.CloseSourceUI = this.MenuItemCloseSourceUI.Checked;
            this.axFiScn1.SourceCurrentScan = this.MenuItemSourceCurrentScan.Checked;
            this.axFiScn1.SilentMode = this.MenuItemSilentMode.Checked;
            this.axFiScn1.Report = (short)ModuleScan.intReport;
            this.axFiScn1.ReportFile = ModuleScan.strReportFile;
            this.axFiScn1.Indicator = MenuItemIndicator.Checked;
            this.axFiScn1.Highlight = short.Parse(txtHighlight.Text);
            this.axFiScn1.Shadow = short.Parse(txtShadow.Text);
            this.axFiScn1.OverScan = (short)this.cboOverScan.SelectedIndex;
            this.axFiScn1.Unit = (short)this.cboUnit.SelectedIndex;
            this.axFiScn1.AutoBright = this.chkAutoBright.Checked;
            this.axFiScn1.AutomaticSenseMedium = this.chkAutomaticSenseMedium.Checked;
            this.axFiScn1.BackgroundSmoothing = (short)this.cboBackgroundSmoothing.SelectedIndex;
            this.axFiScn1.BlankPageSkip = short.Parse(this.txtBlankPageSkip.Text);
            this.axFiScn1.JobControlMode = (short)this.cboJobControlMode.SelectedIndex;
            this.axFiScn1.DTCSensitivity = short.Parse(this.txtDTCSensitivity.Text);
            this.axFiScn1.BackgroundThreshold = short.Parse(this.txtBackgroundThreshold.Text);
            this.axFiScn1.CharacterThickness = short.Parse(this.txtCharacterThickness.Text);
            this.axFiScn1.SDTCSensitivity = short.Parse(this.txtSDTCSensitivity.Text);
            this.axFiScn1.NoiseRejection = short.Parse(this.txtNoiseRejection.Text);
            this.axFiScn1.ADTCThreshold = short.Parse(this.txtADTCThreshold.Text);
            this.axFiScn1.FadingCompensation = short.Parse(this.txtFadingCompensation.Text);
            this.axFiScn1.sRGB = this.chksRGB.Checked;
            this.axFiScn1.PatternRemoval = (short)this.cboPatternRemoval.SelectedIndex;
            this.axFiScn1.Sharpness = (short)this.cboSharpness.SelectedIndex;
            this.axFiScn1.PunchHoleRemoval = (short)this.cboPunchHoleRemoval.SelectedIndex;
            this.axFiScn1.PunchHoleRemovalMode = (short)this.cboPunchHoleRemovalMode.SelectedIndex;
            this.axFiScn1.VerticalLineReduction = this.chkVerticalLineReduction.Checked;
            this.axFiScn1.AIQCNotice = this.chkAIQCNotice.Checked;
            this.axFiScn1.MultiFeedNotice = this.chkMultiFeedNotice.Checked;
            this.axFiScn1.BackgroundSmoothness = short.Parse(this.txtBackgroundSmoothness.Text);
            this.axFiScn1.CropPriority = (short)this.cboCropPriority.SelectedIndex;
            this.axFiScn1.Deskew = (short)this.cboDeskew.SelectedIndex;
            this.axFiScn1.DeskewBackground = (short)this.cboDeskewBackground.SelectedIndex;
            this.axFiScn1.DeskewMode = (short)this.cboDeskewMode.SelectedIndex;
            this.axFiScn1.BlankPageSkipMode = (short)this.cboBlankPageSkipMode.SelectedIndex;
            this.axFiScn1.BlankPageSkipTabPage = (short)this.cboBlankPageSkipTabPage.SelectedIndex;
            this.axFiScn1.BlankPageIgnoreAreaSize = short.Parse(this.txtBlankPageIgnoreAreaSize.Text);
            float cropMarginSize;
            try
            {
                cropMarginSize = float.Parse(txtCropMarginSize.Text);
            }
            catch (FormatException)
            {
                cropMarginSize = float.Parse(txtCropMarginSize.Text.Replace('.', ','));
            }
            this.axFiScn1.CropMarginSize = cropMarginSize;
            this.axFiScn1.SelectOutputSize = (short)this.cboSelectOutputSize.SelectedIndex;
            float multiFeedModeChangeSize;
            try
            {
                multiFeedModeChangeSize = float.Parse(txtMultiFeedModeChangeSize.Text);
            }
            catch (FormatException)
            {
                multiFeedModeChangeSize = float.Parse(txtMultiFeedModeChangeSize.Text.Replace('.', ','));
            }
            this.axFiScn1.MultiFeedModeChangeSize = multiFeedModeChangeSize;
            this.axFiScn1.BlankPageNotice = (short)this.cboBlankPageNotice.SelectedIndex;
            this.axFiScn1.HwCompression = this.chkHwCompression.Checked;
            this.axFiScn1.LengthDetection = (short)this.cboLengthDetection.SelectedIndex;
            this.axFiScn1.FrontBackMergingEnabled = this.chkFrontBackMergingEnabled.Checked;
            this.axFiScn1.FrontBackMergingLocation = (short)this.cboFrontBackMergingLocation.SelectedIndex;
            if (this.cboFrontBackMergingRotation.SelectedIndex == ModuleScan.FBMR_INDEX_R180)
            {
                this.axFiScn1.FrontBackMergingRotation = (short)ModuleScan.FBMR_R180;
            }
            else
            {
                this.axFiScn1.FrontBackMergingRotation = (short)this.cboFrontBackMergingRotation.SelectedIndex;
            }
            this.axFiScn1.FrontBackMergingTarget = (short)this.cboFrontBackMergingTarget.SelectedIndex;
            if (this.cboFrontBackMergingTargetMode.SelectedIndex == ModuleScan.FBMTM_INDEX_CUSTOM)
            {
                this.axFiScn1.FrontBackMergingTargetMode = (short)ModuleScan.FBMTM_CUSTOM;
            }
            else if (this.cboFrontBackMergingTargetMode.SelectedIndex == ModuleScan.FBMTM_INDEX_CARDSIZE)
            {
                this.axFiScn1.FrontBackMergingTargetMode = (short)ModuleScan.FBMTM_CARDSIZE;
            }
            float frontBackMergingTargetSize;
            try
            {
                frontBackMergingTargetSize = float.Parse(txtFrontBackMergingTargetSize.Text);
            }
            catch (FormatException)
            {
                frontBackMergingTargetSize = float.Parse(txtFrontBackMergingTargetSize.Text.Replace('.', ','));
            }
            this.axFiScn1.FrontBackMergingTargetSize = frontBackMergingTargetSize;
            this.axFiScn1.DivideLongPage = this.chkDivideLongPage.Checked;
            this.axFiScn1.CharacterExtraction = this.chkCharacterExtraction.Checked;
            int CharacterExtractionMethod = 0;
            if (this.chkReversedTypeExtraction.Checked)
            {
                CharacterExtractionMethod += ModuleScan.CEM_REVERSEDTYPEEXTRACTION;
            }
            if (this.chkHalftoneRemoval.Checked)
            {
                CharacterExtractionMethod += ModuleScan.CEM_HALFTONEREMOVAL;
            }
            if (this.chkStampRemoval.Checked)
            {
                CharacterExtractionMethod += ModuleScan.CEM_STAMPREMOVAL;
            }
            this.axFiScn1.CharacterExtractionMethod = CharacterExtractionMethod;
            this.axFiScn1.SimpleSlicePatternRemoval = this.chkSimpleSlicePatternRemoval.Checked;
            this.axFiScn1.FrontBackDetection = (short)this.cboFrontBackDetection.SelectedIndex;
            this.axFiScn1.PaperProtection = (short)this.cboPaperProtection.SelectedIndex;

            this.axFiScn1.ColorReproduction = (short)this.cboColorReproduction.SelectedIndex;
            this.axFiScn1.ColorReproductionBrightness = short.Parse(this.txtColorReproductionBrightness.Text);
            this.axFiScn1.ColorReproductionContrast = short.Parse(this.txtColorReproductionContrast.Text);
            this.axFiScn1.ColorReproductionHighlight = short.Parse(txtColorReproductionHighlight.Text);
            this.axFiScn1.ColorReproductionShadow = short.Parse(txtColorReproductionShadow.Text);
            float colorReproductionCustomGammaValue;
            try
            {
                colorReproductionCustomGammaValue = float.Parse(txtColorReproductionCustomGamma.Text);
            }
            catch (FormatException)
            {
                colorReproductionCustomGammaValue = float.Parse(txtColorReproductionCustomGamma.Text.Replace('.', ','));
            }
            this.axFiScn1.ColorReproductionCustomGamma = colorReproductionCustomGammaValue;
            this.axFiScn1.AdjustRGB = this.chkAdjustRGB.Checked;
            this.axFiScn1.AdjustRGBR = short.Parse(txtAdjustRGBR.Text);
            this.axFiScn1.AdjustRGBG = short.Parse(txtAdjustRGBG.Text);
            this.axFiScn1.AdjustRGBB = short.Parse(txtAdjustRGBB.Text);

            this.axFiScn1.BarcodeDetection = this.chkBarcodeDetection.Checked;
            this.axFiScn1.BarcodeDirection = (short)this.cboBarcodeDirection.SelectedIndex;
            float barcodeLeft;
            try
            {
                barcodeLeft = float.Parse(txtBarcodeRegionLeft.Text);
            }
            catch (FormatException)
            {
                barcodeLeft = float.Parse(txtBarcodeRegionLeft.Text.Replace('.', ','));
            }
            this.axFiScn1.BarcodeRegionLeft = barcodeLeft;
            float barcodeTop;
            try
            {
                barcodeTop = float.Parse(txtBarcodeRegionTop.Text);
            }
            catch (FormatException)
            {
                barcodeTop = float.Parse(txtBarcodeRegionTop.Text.Replace('.', ','));
            }
            this.axFiScn1.BarcodeRegionTop = barcodeTop;
            float barcodeWidth;
            try
            {
                barcodeWidth = float.Parse(txtBarcodeRegionWidth.Text);
            }
            catch (FormatException)
            {
                barcodeWidth = float.Parse(txtBarcodeRegionWidth.Text.Replace('.', ','));
            }
            this.axFiScn1.BarcodeRegionWidth = barcodeWidth;
            float barcodeLength;
            try
            {
                barcodeLength = float.Parse(txtBarcodeRegionLength.Text);
            }
            catch (FormatException)
            {
                barcodeLength = float.Parse(txtBarcodeRegionLength.Text.Replace('.', ','));
            }
            this.axFiScn1.BarcodeRegionLength = barcodeLength;
            int barcodeType = 0;
            if (this.chkEAN8.Checked) 
            {
                barcodeType += ModuleScan.BA_EAN8;
            }
            if (this.chkEAN13.Checked) 
            {
                barcodeType += ModuleScan.BA_EAN13;
            }
            if (this.chkCode3of9.Checked) 
            {
                barcodeType += ModuleScan.BA_CODE3OF9;
            }
            if (this.chkCode128.Checked) 
            {
                barcodeType += ModuleScan.BA_CODE128;
            }
            if (this.chkITF.Checked) 
            {
                barcodeType += ModuleScan.BA_ITF;
            }
            if (this.chkUPCA.Checked) 
            {
                barcodeType += ModuleScan.BA_UPCA;
            }
            if (this.chkCodabar.Checked) 
            {
                barcodeType += ModuleScan.BA_CODABAR;
            }
            if (this.chkPDF417.Checked) 
            {
                barcodeType += ModuleScan.BA_PDF417;
            }
            if (this.chkQRCode.Checked) 
            {
                barcodeType += ModuleScan.BA_QRCODE;
            }
            if (this.chkDataMatrix.Checked)
            {
                barcodeType += ModuleScan.BA_DATAMATRIX;
            }
            this.axFiScn1.BarcodeType = barcodeType;
            this.axFiScn1.BarcodeMaxSearchPriorities = short.Parse(this.txtBarcodeMaxSearchPriorities.Text);
            this.axFiScn1.BarcodeNotDetectionNotice = this.chkBarcodeNotDetectionNotice.Checked;

            this.axFiScn1.PatchCodeDetection = this.chkPatchCodeDetection.Checked;
            this.axFiScn1.PatchCodeDirection = (short)this.cboPatchCodeDirection.SelectedIndex;
            int patchCodeType = 0;
            if (this.chkPatch1.Checked) 
            {
                patchCodeType += ModuleScan.PA_PATCH1;
            }
            if (this.chkPatch2.Checked) 
            {
                patchCodeType += ModuleScan.PA_PATCH2;
            }
            if (this.chkPatch3.Checked) 
            {
                patchCodeType += ModuleScan.PA_PATCH3;
            }
            if (this.chkPatch4.Checked) 
            {
                patchCodeType += ModuleScan.PA_PATCH4;
            }
            if (this.chkPatch6.Checked) 
            {
                patchCodeType += ModuleScan.PA_PATCH6;
            }
            if (this.chkPatchT.Checked) 
            {
                patchCodeType += ModuleScan.PA_PATCHT;
            }
            this.axFiScn1.PatchCodeType = patchCodeType;

            this.axFiScn1.EdgeFiller = (short)this.cboEdgeFiller.SelectedIndex;
            float edgeTop;
            try
            {
                edgeTop = float.Parse(txtEdgeFillerTop.Text);
            }
            catch (FormatException)
            {
                edgeTop = float.Parse(txtEdgeFillerTop.Text.Replace('.', ','));
            }
            this.axFiScn1.EdgeFillerTop = edgeTop;
            float edgeBottom;
            try
            {
                edgeBottom = float.Parse(txtEdgeFillerBottom.Text);
            }
            catch (FormatException)
            {
                edgeBottom = float.Parse(txtEdgeFillerBottom.Text.Replace('.', ','));
            }
            this.axFiScn1.EdgeFillerBottom = edgeBottom;
            float edgeLeft;
            try
            {
                edgeLeft = float.Parse(txtEdgeFillerLeft.Text);
            }
            catch (FormatException)
            {
                edgeLeft = float.Parse(txtEdgeFillerLeft.Text.Replace('.', ','));
            }
            this.axFiScn1.EdgeFillerLeft = edgeLeft;
            float edgeRight;
            try
            {
                edgeRight = float.Parse(txtEdgeFillerRight.Text);
            }
            catch (FormatException)
            {
                edgeRight = float.Parse(txtEdgeFillerRight.Text.Replace('.', ','));
            }
            this.axFiScn1.EdgeFillerRight = edgeRight;
            this.axFiScn1.EdgeRepair = (bool)this.chkEdgeRepair.Checked;

            this.axFiScn1.MultiStreamMode = (short)this.cboMultiStreamMode.SelectedIndex;
            this.axFiScn1.MultiStreamFileNameMode = (short)this.cboMultiStreamFileNameMode.SelectedIndex;
            this.axFiScn1.MultiStreamDefaultValueMode = (short)this.cboMultiStreamDefaultValueMode.SelectedIndex;
            this.axFiScn1.FileCounterEx1 = System.Int32.Parse(this.txtFileCounterEx1.Text);
            this.axFiScn1.FileName1 = this.txtFileName1.Text;
            this.axFiScn1.FileCounterEx2 = System.Int32.Parse(this.txtFileCounterEx2.Text);
            this.axFiScn1.FileName2 = this.txtFileName2.Text;
            this.axFiScn1.FileCounterEx3 = System.Int32.Parse(this.txtFileCounterEx3.Text);
            this.axFiScn1.FileName3 = this.txtFileName3.Text;

            this.axFiScn1.AutoProfile = (short)formAutoProfile.cboAutoProfile.SelectedIndex;
            this.axFiScn1.AutoProfileSensitivity = short.Parse(formAutoProfile.txtAutoProfileSensitivity.Text);

            this.axFiScn1.CarrierSheetClippingMode = (short)ModuleScan.intCarrierSheetClippingMode;
            this.axFiScn1.ManualFeedMode = (short)(this.cboManualFeedMode.SelectedIndex + 1);
            this.axFiScn1.StapleDetection = (short)this.cboStapleDetection.SelectedIndex;
            this.axFiScn1.HwAutomaticDeskew = (short)this.cboHwAutomaticDeskew.SelectedIndex;
            this.axFiScn1.HwMoireReductionMode = (short)this.cboHwMoireReductionMode.SelectedIndex;

        }
        //----------------------------------------------------------------------------
        //  Function    : Reading of the parameter information stored in the ini file
        //  Argument    : Nothing
        //  Return code : Nothing
        //----------------------------------------------------------------------------
        private void InitialFileRead()
        {
            int topWnd;
            int leftWnd;

            StringBuilder strWork;
            int rcl;

            if(ModuleScan.strFilePath == null || ModuleScan.strFilePath.Equals("") == true)
            {
                //A default value is used when reading of a ini file goes wrong.
                DefaultSet();
                return;
            }
            
            try
            {
                //Display position information on a dialog
                leftWnd = ModuleScan.GetPrivateProfileInt("Form", "Left", -1, ModuleScan.strFilePath);
                topWnd = ModuleScan.GetPrivateProfileInt("Form", "Top", -1, ModuleScan.strFilePath);
                if(topWnd != -1)
                {
                    this.Top = topWnd;
                }
                if(leftWnd != -1)
                {
                    this.Left = leftWnd;
                }

                //Read configuration parameter
                this.cboScanTo.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "ScanToType", ModuleScan.TYPE_FILE, ModuleScan.strFilePath);
                this.cboFileType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "FileType", ModuleScan.FILE_TIF, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "FileName", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtFileName.Text = strWork.ToString();
                }
                else
                {
                    this.txtFileName.Text = ModuleScan.strDirPath + "\\image########";
                }
                this.cboOverwrite.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Overwrite", ModuleScan.OW_CONFIRM, ModuleScan.strFilePath);
                this.txtFileCounterEx.Text = ModuleScan.GetPrivateProfileInt("Scan", "FileCounterEx", 1, ModuleScan.strFilePath).ToString();
                this.cboCompType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "CompressionType", ModuleScan.COMP_MMR, ModuleScan.strFilePath);
                this.cboJpegQuality.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "JpegQuality", ModuleScan.COMP_JPEG4, ModuleScan.strFilePath);
                this.cboScanMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "ScanMode", ModuleScan.SM_NORMAL, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "ScanContinue", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkScanContinue.Checked = true;
                }
                else
                {
                    this.chkScanContinue.Checked = false;
                }
                this.cboScanContinueMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "ScanContinueMode", 0, ModuleScan.strFilePath);
                this.cboPaperSupply.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PaperSupply", ModuleScan.ADF, ModuleScan.strFilePath);
                this.txtScanCount.Text = ModuleScan.GetPrivateProfileInt("Scan", "ScanCount", -1, ModuleScan.strFilePath).ToString();
                this.cboPaperSize.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PaperSize", ModuleScan.PSIZE_A4, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "LongPage", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkLongPage.Checked = true;
                }
                else
                {
                    this.chkLongPage.Checked = false;
                }
                this.cboOrientation.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Orientation", ModuleScan.PORTRAIT, ModuleScan.strFilePath);
                this.cboUnit.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Unit", 0, ModuleScan.strFilePath);
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "CustomPaperWidth", "8.268", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtCustomPaperWidth.Text = strWork.ToString();
                }
                else
                {
                    this.txtCustomPaperWidth.Text = "8.268";
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "CustomPaperLength", "11.693", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtCustomPaperLength.Text = strWork.ToString();
                }
                else
                {
                    this.txtCustomPaperLength.Text = "11.693";
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "RegionLeft", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0) 
                {
                    txtRegionLeft.Text = strWork.ToString();
                }
                else
                {
                    txtRegionLeft.Text = "0";
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "RegionTop", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0) 
                {
                    txtRegionTop.Text = strWork.ToString();
                }
                else
                {
                    txtRegionTop.Text = "0";
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "RegionWidth", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0) 
                {
                    txtRegionWidth.Text = strWork.ToString();
                }
                else
                {
                    txtRegionWidth.Text = "0";
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "RegionLength", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0) 
                {
                    txtRegionLength.Text = strWork.ToString();
                }
                else
                {
                    txtRegionLength.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "UndefinedScanning", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkUndefinedScanning.Checked = true;
                }
                else
                {
                    this.chkUndefinedScanning.Checked = false;
                }
                this.cboBackgroundColor.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "BackgroundColor", 0, ModuleScan.strFilePath);
                this.cboPixelType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PixelType", ModuleScan.PIXEL_BLACK_WHITE, ModuleScan.strFilePath);
                this.txtAutomaticColorSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "AutomaticColorSensitivity", 0, ModuleScan.strFilePath).ToString();
                this.cboAutomaticColorBackground.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "AutomaticColorBackground", 0, ModuleScan.strFilePath);
                this.cboResolution.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Resolution", ModuleScan.RS_300, ModuleScan.strFilePath);
                this.txtCustomResolution.Text = ModuleScan.GetPrivateProfileInt("Scan", "CustomResolution", 300, ModuleScan.strFilePath).ToString();
                this.txtBrightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Brightness", 128, ModuleScan.strFilePath).ToString();
                this.txtContrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Contrast", 128, ModuleScan.strFilePath).ToString();
                this.txtThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Threshold", 128, ModuleScan.strFilePath).ToString();
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Reverse", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkReverse.Checked = true;
                }
                else
                {
                    this.chkReverse.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Mirroring", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkMirroring.Checked = true;
                }
                else
                {
                    this.chkMirroring.Checked = false;
                }
                this.cboRotation.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Rotation", ModuleScan.RT_NONE, ModuleScan.strFilePath);
                this.cboAutomaticRotateMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "AutomaticRotateMode", ModuleScan.ARM_STANDARD, ModuleScan.strFilePath);
                this.cboBackground.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Background", ModuleScan.MODE_OFF, ModuleScan.strFilePath);
                this.cboOutline.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Outline", ModuleScan.NONE, ModuleScan.strFilePath);
                this.cboHalftone.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Halftone", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "HalftoneFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtHalftoneFile.Text = strWork.ToString();
                }
                this.cboGamma.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Gamma", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "GammaFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtGammaFile.Text = strWork.ToString();
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "CustomGamma", "2.2", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtCustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtCustomGamma.Text = "2.2";
                }
                this.cboAutoSeparation.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Separation", ModuleScan.AS_OFF, ModuleScan.strFilePath);
                this.cboSEE.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "SEE", ModuleScan.SEE_OFF, ModuleScan.strFilePath);
                this.cboThresholdCurve.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "ThresholdCurve", ModuleScan.TH_CURVE1, ModuleScan.strFilePath);
                this.cboNoiseRemoval.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "NoiseRemoval", ModuleScan.NONE, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PreFiltering", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPreFiltering.Checked = true;
                }
                else
                {
                    this.chkPreFiltering.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Smoothing", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkSmoothing.Checked = true;
                }
                else
                {
                    this.chkSmoothing.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Endorser", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkEndorser.Checked = true;
                }
                else
                {
                    this.chkEndorser.Checked = false;
                    this.cboEndorserDialog.Enabled = false;
                    this.txtEndorserCounter.Enabled = false;
                    this.txtEndorserString.Enabled = false;
                    this.txtEndorserOffset.Enabled = false;
                    this.cboEndorserDirection.Enabled = false;
                    this.cboEndorserCountStep.Enabled = false;
                    this.cboEndorserCountDirection.Enabled = false;
                    this.cboEndorserFont.Enabled = false;
                    this.chkSynchronizationDigitalEndorser.Enabled = false;
                }
                this.cboEndorserDialog.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "EndorserDialog", ModuleScan.EDD_OFF, ModuleScan.strFilePath);
                this.txtEndorserCounter.Text = ModuleScan.GetPrivateProfileInt("Scan", "EndorserCounter", 0, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(50);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "EndorserString", "", strWork, 50, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtEndorserString.Text = strWork.ToString();
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "EndorserOffset", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtEndorserOffset.Text = strWork.ToString();
                }
                else
                {
                    txtEndorserOffset.Text = "0";
                }
                this.cboEndorserDirection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "EndorserDirection", 0, ModuleScan.strFilePath);
                this.cboEndorserCountStep.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "EndorserCountStep", 1, ModuleScan.strFilePath);
                this.cboEndorserCountDirection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "EndorserCountDirection", 0, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "DigitalEndorser", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkDigitalEndorser.Checked = true;
                }
                else
                {
                    this.chkDigitalEndorser.Checked = false;
                    this.txtDigitalEndorserCounter.Enabled = false;
                    this.txtDigitalEndorserString.Enabled = false;
                    this.txtDigitalEndorserXOffset.Enabled = false;
                    this.txtDigitalEndorserYOffset.Enabled = false;
                    this.cboDigitalEndorserDirection.Enabled = false;
                    this.cboDigitalEndorserCountStep.Enabled = false;
                    this.cboDigitalEndorserCountDirection.Enabled = false;
                }
                this.txtDigitalEndorserCounter.Text = ModuleScan.GetPrivateProfileInt("Scan", "DigitalEndorserCounter", 0, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(246);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "DigitalEndorserString", "", strWork, 246, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtDigitalEndorserString.Text = strWork.ToString();
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "DigitalEndorserXOffset", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtDigitalEndorserXOffset.Text = strWork.ToString();
                }
                else
                {
                    txtDigitalEndorserXOffset.Text = "0";
                }
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "DigitalEndorserYOffset", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtDigitalEndorserYOffset.Text = strWork.ToString();
                }
                else
                {
                    txtDigitalEndorserYOffset.Text = "0";
                }
                this.cboDigitalEndorserDirection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "DigitalEndorserDirection", 0, ModuleScan.strFilePath);
                this.cboDigitalEndorserCountStep.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "DigitalEndorserCountStep", 1, ModuleScan.strFilePath);
                this.cboDigitalEndorserCountDirection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "DigitalEndorserCountDirection", 0, ModuleScan.strFilePath);
                

                rcl = ModuleScan.GetPrivateProfileInt("Scan", "SynchronizationDigitalEndorser", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkSynchronizationDigitalEndorser.Checked = true;
                }
                else
                {
                    this.chkSynchronizationDigitalEndorser.Checked = false;
                }
                this.cboJobControl.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "JobControl", 0, ModuleScan.strFilePath);
                this.cboBinding.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Binding", 0, ModuleScan.strFilePath);
                this.cboMultiFeed.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "DoubleFeed", 0, ModuleScan.strFilePath);
                this.cboFilter.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Filter", ModuleScan.FILTER_GREEN, ModuleScan.strFilePath);
                this.txtFilterSaturationSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "FilterSaturationSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtSkipWhitePage.Text = ModuleScan.GetPrivateProfileInt("Scan", "SkipWhitePage", 0, ModuleScan.strFilePath).ToString();
                this.txtSkipBlackPage.Text = ModuleScan.GetPrivateProfileInt("Scan", "SkipBlackPage", 0, ModuleScan.strFilePath).ToString();
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "AutoBorderDetection", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkAutoBorderDetection.Checked = true;
                }
                else
                {
                    this.chkAutoBorderDetection.Checked = false;
                }
                this.cboBlankPageNotice.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "BlankPageNotice", 0, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "HwCompression", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkHwCompression.Checked = true;
                }
                else
                {
                    this.chkHwCompression.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "ShowSourceUI", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.MenuItemShowSourceUI.Checked = true;
                }
                else
                {
                    this.MenuItemShowSourceUI.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "CloseSourceUI", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.MenuItemCloseSourceUI.Checked = true;
                }
                else
                {
                    this.MenuItemCloseSourceUI.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "SourceCurrentScan", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.MenuItemSourceCurrentScan.Checked = true;
                }
                else
                {
                    this.MenuItemSourceCurrentScan.Checked = false;
                }
                this.MenuItemTWAINTemplate.Enabled = this.MenuItemSourceCurrentScan.Checked;
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "SilentMode", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.MenuItemSilentMode.Checked = true;
                }
                else
                {
                    this.MenuItemSilentMode.Checked = false;
                }
                ModuleScan.intCarrierSheetClippingMode = ModuleScan.GetPrivateProfileInt("Scan", "CarrierSheetClippingMode", ModuleScan.CSCM_DRIVERSETTING, ModuleScan.strFilePath);
                this.MenuItemCarrierSheetClippingModeContent.Checked = false;
                this.MenuItemCarrierSheetClippingModeEdge.Checked = false;
                this.MenuItemCarrierSheetClippingModeDriverSetting.Checked = false;
                if (ModuleScan.intCarrierSheetClippingMode == 0)
                {
                    this.MenuItemCarrierSheetClippingModeContent.Checked = true;
                }
                else if (ModuleScan.intCarrierSheetClippingMode == 1)
                {
                    this.MenuItemCarrierSheetClippingModeEdge.Checked = true;
                }
                else
                {
                    this.MenuItemCarrierSheetClippingModeDriverSetting.Checked = true;
                }
                ModuleScan.intReport = ModuleScan.GetPrivateProfileInt("Scan", "Report", ModuleScan.RP_OFF, ModuleScan.strFilePath);
                this.MenuItemReportOFF.Checked = false;
                this.MenuItemReportDisplay.Checked = false;
                this.MenuItemReportFile.Checked = false;
                this.MenuItemReportBoth.Checked = false;
                if(ModuleScan.intReport == 0)
                {
                    this.MenuItemReportOFF.Checked = true;
                }
                else if(ModuleScan.intReport == 1)
                {
                    this.MenuItemReportDisplay.Checked = true;
                }
                else if(ModuleScan.intReport == 2)
                {
                    this.MenuItemReportFile.Checked = true;
                }
                else
                {
                    this.MenuItemReportBoth.Checked = true;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Indicator", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.MenuItemIndicator.Checked = true;
                }
                else
                {
                    this.MenuItemIndicator.Checked = false;
                }
                this.txtHighlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Highlight", 230, ModuleScan.strFilePath).ToString();
                this.txtShadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Shadow", 10, ModuleScan.strFilePath).ToString();
                this.cboOverScan.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "OverScan", 0, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "AutomaticSenseMedium", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkAutomaticSenseMedium.Checked = true;
                }
                else
                {
                    this.chkAutomaticSenseMedium.Checked = false;
                }
                this.cboBackgroundSmoothing.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "BackgroundSmoothing", 0, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "AutoBright", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkAutoBright.Checked = true;
                }
                else
                {
                    this.chkAutoBright.Checked = false;
                }
                this.cboEndorserFont.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "EndorserFont", ModuleScan.EDF_HORIZONTAL, ModuleScan.strFilePath);
                this.cboJobControlMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "JobControlMode", ModuleScan.JCM_SPECIAL_DOCUMENT, ModuleScan.strFilePath);
                this.txtBlankPageSkip.Text = ModuleScan.GetPrivateProfileInt("Scan", "BlankPageSkip", 0, ModuleScan.strFilePath).ToString();
                this.txtDTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "DTCSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtBackgroundThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "BackgroundThreshold", 50, ModuleScan.strFilePath).ToString();
                this.txtCharacterThickness.Text = ModuleScan.GetPrivateProfileInt("Scan", "CharacterThickness", 5, ModuleScan.strFilePath).ToString();
                this.txtSDTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "SDTCSensitivity", 2, ModuleScan.strFilePath).ToString();
                this.txtNoiseRejection.Text = ModuleScan.GetPrivateProfileInt("Scan", "NoiseRejection", 0, ModuleScan.strFilePath).ToString();
                this.txtADTCThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "ADTCThreshold", 83, ModuleScan.strFilePath).ToString();
                this.txtFadingCompensation.Text = ModuleScan.GetPrivateProfileInt("Scan", "FadingCompensation", 0, ModuleScan.strFilePath).ToString();
                this.cboSharpness.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Sharpness", ModuleScan.SH_NONE, ModuleScan.strFilePath);
                this.cboPunchHoleRemoval.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PunchHoleRemoval", ModuleScan.PHR_DO_NOT_REMOVE, ModuleScan.strFilePath);
                this.cboPunchHoleRemovalMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PunchHoleRemovalMode", ModuleScan.PHRM_STANDARD, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "sRGB", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chksRGB.Checked = true;
                }
                else
                {
                    this.chksRGB.Checked = false;
                }
                this.cboPatternRemoval.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PatternRemoval", 1, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "VerticalLineReduction", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkVerticalLineReduction.Checked = true;
                }
                else
                {
                    this.chkVerticalLineReduction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "AIQCNotice", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkAIQCNotice.Checked = true;
                }
                else
                {
                    this.chkAIQCNotice.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "MultiFeedNotice", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkMultiFeedNotice.Checked = true;
                }
                else
                {
                    this.chkMultiFeedNotice.Checked = false;
                }
                this.txtBackgroundSmoothness.Text = ModuleScan.GetPrivateProfileInt("Scan", "BackgroundSmoothness", 5, ModuleScan.strFilePath).ToString();
                this.cboCropPriority.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "CropPriority", 0, ModuleScan.strFilePath);
                this.cboDeskew.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Deskew", 2, ModuleScan.strFilePath);
                this.cboDeskewBackground.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "DeskewBackground", 1, ModuleScan.strFilePath);
                this.cboDeskewMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "DeskewMode", 1, ModuleScan.strFilePath);
                this.cboBlankPageSkipMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "BlankPageSkipMode", 0, ModuleScan.strFilePath);
                this.cboBlankPageSkipTabPage.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "BlankPageSkipTabPage", 0, ModuleScan.strFilePath);
                this.txtBlankPageIgnoreAreaSize.Text = ModuleScan.GetPrivateProfileInt("Scan", "BlankPageIgnoreAreaSize", 16, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "CropMarginSize", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtCropMarginSize.Text = strWork.ToString();
                }
                else
                {
                    this.txtCropMarginSize.Text = "0";
                }
                this.cboSelectOutputSize.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "SelectOutputSize", ModuleScan.SOS_MARGIN, ModuleScan.strFilePath);
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "MultiFeedModeChangeSize", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    this.txtMultiFeedModeChangeSize.Text = strWork.ToString();
                }
                else
                {
                    this.txtMultiFeedModeChangeSize.Text = "0";
                }
                this.cboLengthDetection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "LengthDetection", 0, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "FrontBackMergingEnabled", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkFrontBackMergingEnabled.Checked = true;
                }
                else
                {
                    this.chkFrontBackMergingEnabled.Checked = false;
                }
                this.cboFrontBackMergingLocation.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "FrontBackMergingLocation", ModuleScan.FBML_RIGHT, ModuleScan.strFilePath);
                this.cboFrontBackMergingRotation.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "FrontBackMergingRotation", ModuleScan.FBMR_NONE, ModuleScan.strFilePath);
                this.cboFrontBackMergingTarget.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "FrontBackMergingTarget", ModuleScan.FBMT_ALL, ModuleScan.strFilePath);
                this.cboFrontBackMergingTargetMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "FrontBackMergingTargetMode", ModuleScan.FBMTM_INDEX_CUSTOM, ModuleScan.strFilePath);
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "FrontBackMergingTargetSize", ModuleScan.FBMTG_DEFAULT.ToString(), strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtFrontBackMergingTargetSize.Text = strWork.ToString();
                }
                else
                {
                    this.txtFrontBackMergingTargetSize.Text = ModuleScan.FBMTG_DEFAULT.ToString();
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "DivideLongPage", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkDivideLongPage.Checked = true;
                }
                else
                {
                    this.chkDivideLongPage.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "CharacterExtraction", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkCharacterExtraction.Checked = true;
                }
                else
                {
                    this.chkCharacterExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "ReversedTypeExtraction", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkReversedTypeExtraction.Checked = true;
                }
                else
                {
                    this.chkReversedTypeExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "HalftoneRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkHalftoneRemoval.Checked = true;
                }
                else
                {
                    this.chkHalftoneRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "StampRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStampRemoval.Checked = true;
                }
                else
                {
                    this.chkStampRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "SimpleSlicePatternRemoval", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkSimpleSlicePatternRemoval.Checked = true;
                }
                else
                {
                    this.chkSimpleSlicePatternRemoval.Checked = false;
                }
                this.cboFrontBackDetection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "FrontBackDetection", ModuleScan.FBD_NONE, ModuleScan.strFilePath);
                this.cboPaperProtection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PaperProtection", ModuleScan.PP_DRIVERSETTING, ModuleScan.strFilePath);

                this.cboColorReproduction.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "ColorReproduction", ModuleScan.CR_CONTRAST, ModuleScan.strFilePath);
                this.txtColorReproductionBrightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "ColorReproductionBrightness", 128, ModuleScan.strFilePath).ToString();
                this.txtColorReproductionContrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "ColorReproductionContrast", 128, ModuleScan.strFilePath).ToString();
                this.txtColorReproductionHighlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "ColorReproductionHighlight", 255, ModuleScan.strFilePath).ToString();
                this.txtColorReproductionShadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "ColorReproductionShadow", 0, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "ColorReproductionCustomGamma", "1.0", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtColorReproductionCustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtColorReproductionCustomGamma.Text = "1.0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "AdjustRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkAdjustRGB.Checked = true;
                }
                else
                {
                    this.chkAdjustRGB.Checked = false;
                }
                this.txtAdjustRGBR.Text = ModuleScan.GetPrivateProfileInt("Scan", "AdjustRGBR", 128, ModuleScan.strFilePath).ToString();
                this.txtAdjustRGBG.Text = ModuleScan.GetPrivateProfileInt("Scan", "AdjustRGBG", 128, ModuleScan.strFilePath).ToString();
                this.txtAdjustRGBB.Text = ModuleScan.GetPrivateProfileInt("Scan", "AdjustRGBB", 128, ModuleScan.strFilePath).ToString();

                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeDetection", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkBarcodeDetection.Checked = true;
                }
                else
                {
                    this.chkBarcodeDetection.Checked = false;
                    this.cboBarcodeDirection.Enabled = false;
                    this.txtBarcodeRegionLeft.Enabled = false;
                    this.txtBarcodeRegionTop.Enabled = false;
                    this.txtBarcodeRegionWidth.Enabled = false;
                    this.txtBarcodeRegionLength.Enabled = false;
                    this.chkEAN8.Enabled = false;
                    this.chkEAN13.Enabled = false;
                    this.chkCode3of9.Enabled = false;
                    this.chkCode128.Enabled = false;
                    this.chkITF.Enabled = false;
                    this.chkUPCA.Enabled = false;
                    this.chkCodabar.Enabled = false;
                    this.chkPDF417.Enabled = false;
                    this.chkQRCode.Enabled = false;
                    this.chkDataMatrix.Enabled = false;
                    this.txtBarcodeMaxSearchPriorities.Enabled = false;
                    this.chkBarcodeNotDetectionNotice.Enabled = false;
                }
                this.cboBarcodeDirection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeDirection", ModuleScan.BD_HORIZONTAL_VERTICAL, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "BarcodeRegionLeft", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtBarcodeRegionLeft.Text = strWork.ToString();
                }
                else
                {
                    txtBarcodeRegionLeft.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileString("Scan", "BarcodeRegionTop", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtBarcodeRegionTop.Text = strWork.ToString();
                }
                else
                {
                    txtBarcodeRegionTop.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileString("Scan", "BarcodeRegionWidth", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtBarcodeRegionWidth.Text = strWork.ToString();
                }
                else
                {
                    txtBarcodeRegionWidth.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileString("Scan", "BarcodeRegionLength", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtBarcodeRegionLength.Text = strWork.ToString();
                }
                else
                {
                    txtBarcodeRegionLength.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_EAN8", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkEAN8.Checked = true;
                }
                else
                {
                    this.chkEAN8.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_EAN13", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkEAN13.Checked = true;
                }
                else
                {
                    this.chkEAN13.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_Code3of9", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkCode3of9.Checked = true;
                }
                else
                {
                    this.chkCode3of9.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_Code128", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkCode128.Checked = true;
                }
                else
                {
                    this.chkCode128.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_ITF", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkITF.Checked = true;
                }
                else
                {
                    this.chkITF.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_UPC-A", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkUPCA.Checked = true;
                }
                else
                {
                    this.chkUPCA.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_Codabar", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkCodabar.Checked = true;
                }
                else
                {
                    this.chkCodabar.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_PDF417", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPDF417.Checked = true;
                }
                else
                {
                    this.chkPDF417.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_QRCode", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkQRCode.Checked = true;
                }
                else
                {
                    this.chkQRCode.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeType_DataMatrix", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkDataMatrix.Checked = true;
                }
                else
                {
                    this.chkDataMatrix.Checked = false;
                }
                this.txtBarcodeMaxSearchPriorities.Text = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeMaxSearchPriorities", 1, ModuleScan.strFilePath).ToString();
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "BarcodeNotDetectionNotice", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkBarcodeNotDetectionNotice.Checked = true;
                }
                else
                {
                    this.chkBarcodeNotDetectionNotice.Checked = false;
                }

                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeDetection", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatchCodeDetection.Checked = true;
                }
                else
                {
                    this.chkPatchCodeDetection.Checked = false;
                }
                this.cboPatchCodeDirection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeDirection", ModuleScan.PD_VERTICAL, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeType_Patch1", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatch1.Checked = true;
                }
                else
                {
                    this.chkPatch1.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeType_Patch2", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatch2.Checked = true;
                }
                else
                {
                    this.chkPatch2.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeType_Patch3", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatch3.Checked = true;
                }
                else
                {
                    this.chkPatch3.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeType_Patch4", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatch4.Checked = true;
                }
                else
                {
                    this.chkPatch4.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeType_Patch6", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatch6.Checked = true;
                }
                else
                {
                    this.chkPatch6.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "PatchCodeType_PatchT", 1, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkPatchT.Checked = true;
                }
                else
                {
                    this.chkPatchT.Checked = false;
                }

                this.cboEdgeFiller.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "EdgeFiller", ModuleScan.EF_OFF, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "EdgeFillerTop", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtEdgeFillerTop.Text = strWork.ToString();
                }
                else
                {
                    txtEdgeFillerTop.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileString("Scan", "EdgeFillerBottom", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtEdgeFillerBottom.Text = strWork.ToString();
                }
                else
                {
                    txtEdgeFillerBottom.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileString("Scan", "EdgeFillerLeft", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtEdgeFillerLeft.Text = strWork.ToString();
                }
                else
                {
                    txtEdgeFillerLeft.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileString("Scan", "EdgeFillerRight", "0", strWork, 10, ModuleScan.strFilePath);
                if(rcl > 0)
                {
                    txtEdgeFillerRight.Text = strWork.ToString();
                }
                else
                {
                    txtEdgeFillerRight.Text = "0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "EdgeRepair", 0, ModuleScan.strFilePath);
                if(rcl == 1)
                {
                    this.chkEdgeRepair.Checked = true;
                }
                else
                {
                    this.chkEdgeRepair.Checked = false;
                }

                this.cboMultiStreamMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "MultiStreamMode", ModuleScan.MSM_OFF, ModuleScan.strFilePath);
                this.cboMultiStreamFileNameMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "MultiStreamFileNameMode", ModuleScan.MSFNM_OFF, ModuleScan.strFilePath);
                this.cboMultiStreamDefaultValueMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "MultiStreamDefaultValueMode", ModuleScan.MSDVM_OFF, ModuleScan.strFilePath);
                this.cboStream1PixelType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1PixelType", ModuleScan.PIXEL_BLACK_WHITE, ModuleScan.strFilePath);
                this.cboStream1FileType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1FileType", ModuleScan.FILE_TIF, ModuleScan.strFilePath);
                this.cboStream1CompressionType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1CompressionType", ModuleScan.COMP_MMR, ModuleScan.strFilePath);
                this.cboStream1Resolution.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Resolution", ModuleScan.RS_300, ModuleScan.strFilePath);
                this.txtStream1CustomResolution.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1CustomResolution", 300, ModuleScan.strFilePath).ToString();
                this.txtFileCounterEx1.Text = ModuleScan.GetPrivateProfileInt("Scan", "FileCounterEx1", 1, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "FileName1", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtFileName1.Text = strWork.ToString();
                }
                else
                {
                    this.txtFileName1.Text = ModuleScan.strDirPath + "\\image1_########";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1AutoBright", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1AutoBright.Checked = true;
                }
                else
                {
                    this.chkStream1AutoBright.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Reverse", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1Reverse.Checked = true;
                }
                else
                {
                    this.chkStream1Reverse.Checked = false;
                }
                this.cboStream1Gamma.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Gamma", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream1GammaFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream1GammaFile.Text = strWork.ToString();
                }
                this.txtStream1Brightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Brightness", 128, ModuleScan.strFilePath).ToString();
                this.txtStream1Contrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Contrast", 128, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream1CustomGamma", "2.2", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream1CustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtStream1CustomGamma.Text = "2.2";
                }
                this.cboStream1Background.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Background", ModuleScan.MODE_OFF, ModuleScan.strFilePath);
                this.cboStream1Sharpness.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Sharpness", ModuleScan.SH_NONE, ModuleScan.strFilePath);
                this.txtStream1Threshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Threshold", 128, ModuleScan.strFilePath).ToString();
                this.txtStream1DTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1DTCSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtStream1BackgroundThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1BackgroundThreshold", 50, ModuleScan.strFilePath).ToString();
                this.txtStream1CharacterThickness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1CharacterThickness", 5, ModuleScan.strFilePath).ToString();
                this.txtStream1FadingCompensation.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1FadingCompensation", 0, ModuleScan.strFilePath).ToString();
                this.txtStream1NoiseRejection.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1NoiseRejection", 0, ModuleScan.strFilePath).ToString();
                this.cboStream1PatternRemoval.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1PatternRemoval", 1, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1CharacterExtraction", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1CharacterExtraction.Checked = true;
                }
                else
                {
                    this.chkStream1CharacterExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ReversedTypeExtraction", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1ReversedTypeExtraction.Checked = true;
                }
                else
                {
                    this.chkStream1ReversedTypeExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1HalftoneRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1HalftoneRemoval.Checked = true;
                }
                else
                {
                    this.chkStream1HalftoneRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1StampRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1StampRemoval.Checked = true;
                }
                else
                {
                    this.chkStream1StampRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1SimpleSlicePatternRemoval", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1SimpleSlicePatternRemoval.Checked = true;
                }
                else
                {
                    this.chkStream1SimpleSlicePatternRemoval.Checked = false;
                }
                this.txtStream1ADTCThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ADTCThreshold", 83, ModuleScan.strFilePath).ToString();
                this.txtStream1SDTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1SDTCSensitivity", 2, ModuleScan.strFilePath).ToString();
                this.cboStream1Halftone.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Halftone", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream1HalftoneFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream1HalftoneFile.Text = strWork.ToString();
                }
                this.cboStream1SEE.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1SEE", ModuleScan.SEE_OFF, ModuleScan.strFilePath);
                this.cboStream1Filter.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Filter", ModuleScan.FILTER_GREEN, ModuleScan.strFilePath);
                this.txtStream1FilterSaturationSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1FilterSaturationSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtStream1Highlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Highlight", 230, ModuleScan.strFilePath).ToString();
                this.txtStream1Shadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1Shadow", 10, ModuleScan.strFilePath).ToString();
                this.cboStream1BackgroundSmoothing.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1BackgroundSmoothing", 0, ModuleScan.strFilePath);
                this.txtStream1BackgroundSmoothness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1BackgroundSmoothness", 5, ModuleScan.strFilePath).ToString();
                this.cboStream1ColorReproduction.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ColorReproduction", ModuleScan.CR_CONTRAST, ModuleScan.strFilePath);
                this.txtStream1ColorReproductionBrightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ColorReproductionBrightness", 128, ModuleScan.strFilePath).ToString();
                this.txtStream1ColorReproductionContrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ColorReproductionContrast", 128, ModuleScan.strFilePath).ToString();
                this.txtStream1ColorReproductionHighlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ColorReproductionHighlight", 255, ModuleScan.strFilePath).ToString();
                this.txtStream1ColorReproductionShadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1ColorReproductionShadow", 0, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream1ColorReproductionCustomGamma", "1.0", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream1ColorReproductionCustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtStream1ColorReproductionCustomGamma.Text = "1.0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1AdjustRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1AdjustRGB.Checked = true;
                }
                else
                {
                    this.chkStream1AdjustRGB.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream1sRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream1sRGB.Checked = true;
                }
                else
                {
                    this.chkStream1sRGB.Checked = false;
                }
                this.txtStream1AdjustRGBR.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1AdjustRGBR", 128, ModuleScan.strFilePath).ToString();
                this.txtStream1AdjustRGBG.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1AdjustRGBG", 128, ModuleScan.strFilePath).ToString();
                this.txtStream1AdjustRGBB.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream1AdjustRGBB", 128, ModuleScan.strFilePath).ToString();
                this.cboStream2PixelType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2PixelType", ModuleScan.PIXEL_BLACK_WHITE, ModuleScan.strFilePath);
                this.cboStream2FileType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2FileType", ModuleScan.FILE_TIF, ModuleScan.strFilePath);
                this.cboStream2CompressionType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2CompressionType", ModuleScan.COMP_MMR, ModuleScan.strFilePath);
                this.cboStream2Resolution.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Resolution", ModuleScan.RS_300, ModuleScan.strFilePath);
                this.txtStream2CustomResolution.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2CustomResolution", 300, ModuleScan.strFilePath).ToString();
                this.txtFileCounterEx2.Text = ModuleScan.GetPrivateProfileInt("Scan", "FileCounterEx2", 1, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "FileName2", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtFileName2.Text = strWork.ToString();
                }
                else
                {
                    this.txtFileName2.Text = ModuleScan.strDirPath + "\\image2_########";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2AutoBright", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2AutoBright.Checked = true;
                }
                else
                {
                    this.chkStream2AutoBright.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Reverse", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2Reverse.Checked = true;
                }
                else
                {
                    this.chkStream2Reverse.Checked = false;
                }
                this.cboStream2Gamma.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Gamma", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream2GammaFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream2GammaFile.Text = strWork.ToString();
                }
                this.txtStream2Brightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Brightness", 128, ModuleScan.strFilePath).ToString();
                this.txtStream2Contrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Contrast", 128, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream2CustomGamma", "2.2", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream2CustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtStream2CustomGamma.Text = "2.2";
                }
                this.cboStream2Background.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Background", ModuleScan.MODE_OFF, ModuleScan.strFilePath);
                this.cboStream2Sharpness.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Sharpness", ModuleScan.SH_NONE, ModuleScan.strFilePath);
                this.txtStream2Threshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Threshold", 128, ModuleScan.strFilePath).ToString();
                this.txtStream2DTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2DTCSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtStream2BackgroundThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2BackgroundThreshold", 50, ModuleScan.strFilePath).ToString();
                this.txtStream2CharacterThickness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2CharacterThickness", 5, ModuleScan.strFilePath).ToString();
                this.txtStream2FadingCompensation.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2FadingCompensation", 0, ModuleScan.strFilePath).ToString();
                this.txtStream2NoiseRejection.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2NoiseRejection", 0, ModuleScan.strFilePath).ToString();
                this.cboStream2PatternRemoval.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2PatternRemoval", 1, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2CharacterExtraction", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2CharacterExtraction.Checked = true;
                }
                else
                {
                    this.chkStream2CharacterExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ReversedTypeExtraction", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2ReversedTypeExtraction.Checked = true;
                }
                else
                {
                    this.chkStream2ReversedTypeExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2HalftoneRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2HalftoneRemoval.Checked = true;
                }
                else
                {
                    this.chkStream2HalftoneRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2StampRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2StampRemoval.Checked = true;
                }
                else
                {
                    this.chkStream2StampRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2SimpleSlicePatternRemoval", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2SimpleSlicePatternRemoval.Checked = true;
                }
                else
                {
                    this.chkStream2SimpleSlicePatternRemoval.Checked = false;
                }
                this.txtStream2ADTCThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ADTCThreshold", 83, ModuleScan.strFilePath).ToString();
                this.txtStream2SDTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2SDTCSensitivity", 2, ModuleScan.strFilePath).ToString();
                this.cboStream2Halftone.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Halftone", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream2HalftoneFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream2HalftoneFile.Text = strWork.ToString();
                }
                this.cboStream2SEE.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2SEE", ModuleScan.SEE_OFF, ModuleScan.strFilePath);
                this.cboStream2Filter.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Filter", ModuleScan.FILTER_GREEN, ModuleScan.strFilePath);
                this.txtStream2FilterSaturationSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2FilterSaturationSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtStream2Highlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Highlight", 230, ModuleScan.strFilePath).ToString();
                this.txtStream2Shadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2Shadow", 10, ModuleScan.strFilePath).ToString();
                this.cboStream2BackgroundSmoothing.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2BackgroundSmoothing", 0, ModuleScan.strFilePath);
                this.txtStream2BackgroundSmoothness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2BackgroundSmoothness", 5, ModuleScan.strFilePath).ToString();
                this.cboStream2ColorReproduction.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ColorReproduction", ModuleScan.CR_CONTRAST, ModuleScan.strFilePath);
                this.txtStream2ColorReproductionBrightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ColorReproductionBrightness", 128, ModuleScan.strFilePath).ToString();
                this.txtStream2ColorReproductionContrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ColorReproductionContrast", 128, ModuleScan.strFilePath).ToString();
                this.txtStream2ColorReproductionHighlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ColorReproductionHighlight", 255, ModuleScan.strFilePath).ToString();
                this.txtStream2ColorReproductionShadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2ColorReproductionShadow", 0, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream2ColorReproductionCustomGamma", "1.0", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream2ColorReproductionCustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtStream2ColorReproductionCustomGamma.Text = "1.0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2AdjustRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2AdjustRGB.Checked = true;
                }
                else
                {
                    this.chkStream2AdjustRGB.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream2sRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream2sRGB.Checked = true;
                }
                else
                {
                    this.chkStream2sRGB.Checked = false;
                }
                this.txtStream2AdjustRGBR.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2AdjustRGBR", 128, ModuleScan.strFilePath).ToString();
                this.txtStream2AdjustRGBG.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2AdjustRGBG", 128, ModuleScan.strFilePath).ToString();
                this.txtStream2AdjustRGBB.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream2AdjustRGBB", 128, ModuleScan.strFilePath).ToString();
                this.cboStream3PixelType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3PixelType", ModuleScan.PIXEL_BLACK_WHITE, ModuleScan.strFilePath);
                this.cboStream3FileType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3FileType", ModuleScan.FILE_TIF, ModuleScan.strFilePath);
                this.cboStream3CompressionType.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3CompressionType", ModuleScan.COMP_MMR, ModuleScan.strFilePath);
                this.cboStream3Resolution.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Resolution", ModuleScan.RS_300, ModuleScan.strFilePath);
                this.txtStream3CustomResolution.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3CustomResolution", 300, ModuleScan.strFilePath).ToString();
                this.txtFileCounterEx3.Text = ModuleScan.GetPrivateProfileInt("Scan", "FileCounterEx3", 1, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "FileName3", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtFileName3.Text = strWork.ToString();
                }
                else
                {
                    this.txtFileName3.Text = ModuleScan.strDirPath + "\\image3_########";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3AutoBright", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3AutoBright.Checked = true;
                }
                else
                {
                    this.chkStream3AutoBright.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Reverse", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3Reverse.Checked = true;
                }
                else
                {
                    this.chkStream3Reverse.Checked = false;
                }
                this.cboStream3Gamma.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Gamma", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream3GammaFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream3GammaFile.Text = strWork.ToString();
                }
                this.txtStream3Brightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Brightness", 128, ModuleScan.strFilePath).ToString();
                this.txtStream3Contrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Contrast", 128, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream3CustomGamma", "2.2", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream3CustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtStream3CustomGamma.Text = "2.2";
                }
                this.cboStream3Background.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Background", ModuleScan.MODE_OFF, ModuleScan.strFilePath);
                this.cboStream3Sharpness.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Sharpness", ModuleScan.SH_NONE, ModuleScan.strFilePath);
                this.txtStream3Threshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Threshold", 128, ModuleScan.strFilePath).ToString();
                this.txtStream3DTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3DTCSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtStream3BackgroundThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3BackgroundThreshold", 50, ModuleScan.strFilePath).ToString();
                this.txtStream3CharacterThickness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3CharacterThickness", 5, ModuleScan.strFilePath).ToString();
                this.txtStream3FadingCompensation.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3FadingCompensation", 0, ModuleScan.strFilePath).ToString();
                this.txtStream3NoiseRejection.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3NoiseRejection", 0, ModuleScan.strFilePath).ToString();
                this.cboStream3PatternRemoval.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3PatternRemoval", 1, ModuleScan.strFilePath);
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3CharacterExtraction", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3CharacterExtraction.Checked = true;
                }
                else
                {
                    this.chkStream3CharacterExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ReversedTypeExtraction", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3ReversedTypeExtraction.Checked = true;
                }
                else
                {
                    this.chkStream3ReversedTypeExtraction.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3HalftoneRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3HalftoneRemoval.Checked = true;
                }
                else
                {
                    this.chkStream3HalftoneRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3StampRemoval", 1, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3StampRemoval.Checked = true;
                }
                else
                {
                    this.chkStream3StampRemoval.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3SimpleSlicePatternRemoval", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3SimpleSlicePatternRemoval.Checked = true;
                }
                else
                {
                    this.chkStream3SimpleSlicePatternRemoval.Checked = false;
                }
                this.txtStream3ADTCThreshold.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ADTCThreshold", 83, ModuleScan.strFilePath).ToString();
                this.txtStream3SDTCSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3SDTCSensitivity", 2, ModuleScan.strFilePath).ToString();
                this.cboStream3Halftone.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Halftone", ModuleScan.NONE, ModuleScan.strFilePath);
                strWork = new StringBuilder(ModuleScan.MAX_PATH);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream3HalftoneFile", "", strWork, ModuleScan.MAX_PATH, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream3HalftoneFile.Text = strWork.ToString();
                }
                this.cboStream3SEE.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3SEE", ModuleScan.SEE_OFF, ModuleScan.strFilePath);
                this.cboStream3Filter.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Filter", ModuleScan.FILTER_GREEN, ModuleScan.strFilePath);
                this.txtStream3FilterSaturationSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3FilterSaturationSensitivity", 50, ModuleScan.strFilePath).ToString();
                this.txtStream3Highlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Highlight", 230, ModuleScan.strFilePath).ToString();
                this.txtStream3Shadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3Shadow", 10, ModuleScan.strFilePath).ToString();
                this.cboStream3BackgroundSmoothing.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3BackgroundSmoothing", 0, ModuleScan.strFilePath);
                this.txtStream3BackgroundSmoothness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3BackgroundSmoothness", 5, ModuleScan.strFilePath).ToString();
                this.cboStream3ColorReproduction.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ColorReproduction", ModuleScan.CR_CONTRAST, ModuleScan.strFilePath);
                this.txtStream3ColorReproductionBrightness.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ColorReproductionBrightness", 128, ModuleScan.strFilePath).ToString();
                this.txtStream3ColorReproductionContrast.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ColorReproductionContrast", 128, ModuleScan.strFilePath).ToString();
                this.txtStream3ColorReproductionHighlight.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ColorReproductionHighlight", 255, ModuleScan.strFilePath).ToString();
                this.txtStream3ColorReproductionShadow.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3ColorReproductionShadow", 0, ModuleScan.strFilePath).ToString();
                strWork = new StringBuilder(10);
                rcl = ModuleScan.GetPrivateProfileString("Scan", "Stream3ColorReproductionCustomGamma", "1.0", strWork, 10, ModuleScan.strFilePath);
                if (rcl > 0)
                {
                    this.txtStream3ColorReproductionCustomGamma.Text = strWork.ToString();
                }
                else
                {
                    this.txtStream3ColorReproductionCustomGamma.Text = "1.0";
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3AdjustRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3AdjustRGB.Checked = true;
                }
                else
                {
                    this.chkStream3AdjustRGB.Checked = false;
                }
                rcl = ModuleScan.GetPrivateProfileInt("Scan", "Stream3sRGB", 0, ModuleScan.strFilePath);
                if (rcl == 1)
                {
                    this.chkStream3sRGB.Checked = true;
                }
                else
                {
                    this.chkStream3sRGB.Checked = false;
                }
                this.txtStream3AdjustRGBR.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3AdjustRGBR", 128, ModuleScan.strFilePath).ToString();
                this.txtStream3AdjustRGBG.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3AdjustRGBG", 128, ModuleScan.strFilePath).ToString();
                this.txtStream3AdjustRGBB.Text = ModuleScan.GetPrivateProfileInt("Scan", "Stream3AdjustRGBB", 128, ModuleScan.strFilePath).ToString();

                formAutoProfile.cboAutoProfile.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "AutoProfile", ModuleScan.AP_DISABLED, ModuleScan.strFilePath);
                formAutoProfile.txtAutoProfileSensitivity.Text = ModuleScan.GetPrivateProfileInt("Scan", "AutoProfileSensitivity", 3, ModuleScan.strFilePath).ToString();
                this.cboManualFeedMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "ManualFeedMode ", ModuleScan.MFM_HARDWARESETTING, ModuleScan.strFilePath);
                this.cboStapleDetection.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "StapleDetection", ModuleScan.SD_ON, ModuleScan.strFilePath);
                this.cboHwAutomaticDeskew.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "HwAutomaticDeskew", ModuleScan.HAMD_ON, ModuleScan.strFilePath);
                this.cboHwMoireReductionMode.SelectedIndex = ModuleScan.GetPrivateProfileInt("Scan", "HwMoireReductionMode", ModuleScan.HMRM_DRIVERSETTING, ModuleScan.strFilePath);

                ModuleScan.bInitialFileRead = true;
                CarrierSheetSet();
            }
            catch (System.Exception)
            {
                //A default value is used when reading of a ini file goes wrong.
                DefaultSet();
            }
            
        }

        //----------------------------------------------------------------------------
        //  Function    : Writing to the ini file of parameter information
        //  Argument    : Nothing
        //  Return code : Nothing
        //----------------------------------------------------------------------------
        private void  WriteScanINIFile()
        {
            if(ModuleScan.strFilePath == null || ModuleScan.strFilePath.Equals("") == true)
            {
                MessageBox.Show("It cannot write in a file.", "fiScanTest");
                return;
            }

            try
            {
                //Display position information on a dialog
                ModuleScan.WritePrivateProfileString("Form", "Left", this.Left.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Form", "Top", this.Top.ToString(), ModuleScan.strFilePath);

                //Write configuration parameter
                ModuleScan.WritePrivateProfileString("Scan", "ScanToType", this.cboScanTo.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileType", this.cboFileType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileName", this.txtFileName.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Overwrite", this.cboOverwrite.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileCounterEx", this.txtFileCounterEx.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CompressionType", this.cboCompType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "JpegQuality", this.cboJpegQuality.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ScanMode", this.cboScanMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if (this.chkScanContinue.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "ScanContinue", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "ScanContinue", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "ScanContinueMode", this.cboScanContinueMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "PaperSupply", this.cboPaperSupply.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ScanCount", this.txtScanCount.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "PaperSize", this.cboPaperSize.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if(this.chkLongPage.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "LongPage", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "LongPage", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Orientation", this.cboOrientation.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CustomPaperWidth", double.Parse(this.txtCustomPaperWidth.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CustomPaperLength", double.Parse(this.txtCustomPaperLength.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "RegionLeft", double.Parse(this.txtRegionLeft.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "RegionTop", double.Parse(this.txtRegionTop.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "RegionWidth", double.Parse(this.txtRegionWidth.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "RegionLength", double.Parse(this.txtRegionLength.Text).ToString("F3"), ModuleScan.strFilePath);
                if(chkUndefinedScanning.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "UndefinedScanning", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "UndefinedScanning", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "BackgroundColor", this.cboBackgroundColor.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "PixelType", this.cboPixelType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AutomaticColorSensitivity", this.txtAutomaticColorSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AutomaticColorBackground", this.cboAutomaticColorBackground.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Resolution", this.cboResolution.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CustomResolution", this.txtCustomResolution.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Brightness", this.txtBrightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Contrast", this.txtContrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Threshold", this.txtThreshold.Text, ModuleScan.strFilePath);
                if(this.chkReverse.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Reverse", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Reverse", "0", ModuleScan.strFilePath);
                }
                if(chkMirroring.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Mirroring", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Mirroring", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Rotation", this.cboRotation.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AutomaticRotateMode", this.cboAutomaticRotateMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Background", this.cboBackground.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Outline", this.cboOutline.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Halftone", this.cboHalftone.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "HalftoneFile", this.txtHalftoneFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Gamma", this.cboGamma.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "GammaFile", this.txtGammaFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CustomGamma", this.txtCustomGamma.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AutoSeparation", this.cboAutoSeparation.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "SEE", this.cboSEE.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ThresholdCurve", this.cboThresholdCurve.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "NoiseRemoval", this.cboNoiseRemoval.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if(this.chkPreFiltering.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PreFiltering", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PreFiltering", "0", ModuleScan.strFilePath);
                }
                if(this.chkSmoothing.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Smoothing", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Smoothing", "0", ModuleScan.strFilePath);
                }
                if(this.chkEndorser.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Endorser", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Endorser", "0", ModuleScan.strFilePath);
                }

                ModuleScan.WritePrivateProfileString("Scan", "EndorserDialog", this.cboEndorserDialog.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserCounter", this.txtEndorserCounter.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserString", this.txtEndorserString.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserOffset", double.Parse(this.txtEndorserOffset.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserDirection", this.cboEndorserDirection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserCountStep", this.cboEndorserCountStep.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserCountDirection", this.cboEndorserCountDirection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EndorserFont", this.cboEndorserFont.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if(this.chkSynchronizationDigitalEndorser.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SynchronizationDigitalEndorser", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SynchronizationDigitalEndorser", "0", ModuleScan.strFilePath);
                }
                if(this.chkDigitalEndorser.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorser", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorser", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserCounter", this.txtDigitalEndorserCounter.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserString", this.txtDigitalEndorserString.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserXOffset", double.Parse(this.txtDigitalEndorserXOffset.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserYOffset", double.Parse(this.txtDigitalEndorserYOffset.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserDirection", this.cboDigitalEndorserDirection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserCountStep", this.cboDigitalEndorserCountStep.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DigitalEndorserCountDirection", this.cboDigitalEndorserCountDirection.SelectedIndex.ToString(), ModuleScan.strFilePath);

                ModuleScan.WritePrivateProfileString("Scan", "JobControl", this.cboJobControl.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Binding", this.cboBinding.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DoubleFeed", this.cboMultiFeed.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Filter", this.cboFilter.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FilterSaturationSensitivity", this.txtFilterSaturationSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "SkipWhitePage", this.txtSkipWhitePage.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "SkipBlackPage", this.txtSkipBlackPage.Text, ModuleScan.strFilePath);
                if(this.chkAutomaticSenseMedium.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AutomaticSenseMedium", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AutomaticSenseMedium", "0", ModuleScan.strFilePath);
                }
                if(this.chkAutoBright.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AutoBright", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AutoBright", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "BackgroundSmoothing", this.cboBackgroundSmoothing.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BlankPageSkip", this.txtBlankPageSkip.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "JobControlMode", this.cboJobControlMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DTCSensitivity", this.txtDTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BackgroundThreshold", this.txtBackgroundThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CharacterThickness", this.txtCharacterThickness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "SDTCSensitivity", this.txtSDTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "NoiseRejection", this.txtNoiseRejection.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ADTCThreshold", this.txtADTCThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FadingCompensation", this.txtFadingCompensation.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Sharpness", this.cboSharpness.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "PunchHoleRemoval", this.cboPunchHoleRemoval.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "PunchHoleRemovalMode", this.cboPunchHoleRemovalMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if(this.chksRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "sRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "sRGB", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "PatternRemoval", this.cboPatternRemoval.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if(this.chkVerticalLineReduction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "VerticalLineReduction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "VerticalLineReduction", "0", ModuleScan.strFilePath);
                }
                if(this.chkAIQCNotice.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AIQCNotice", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AIQCNotice", "0", ModuleScan.strFilePath);
                }
                if (this.chkMultiFeedNotice.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "MultiFeedNotice", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "MultiFeedNotice", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "BackgroundSmoothness", this.txtBackgroundSmoothness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CropPriority", this.cboCropPriority.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Deskew", this.cboDeskew.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DeskewBackground", this.cboDeskewBackground.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "DeskewMode", this.cboDeskewMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BlankPageSkipMode", this.cboBlankPageSkipMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BlankPageSkipTabPage", this.cboBlankPageSkipTabPage.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BlankPageIgnoreAreaSize", this.txtBlankPageIgnoreAreaSize.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "CropMarginSize", double.Parse(this.txtCropMarginSize.Text).ToString("F1"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "SelectOutputSize", this.cboSelectOutputSize.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "MultiFeedModeChangeSize", double.Parse(this.txtMultiFeedModeChangeSize.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "LengthDetection", this.cboLengthDetection.SelectedIndex.ToString(), ModuleScan.strFilePath);

                if(this.chkAutoBorderDetection.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AutoBorderDetection", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AutoBorderDetection", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "BlankPageNotice", this.cboBlankPageNotice.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if (this.chkHwCompression.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "HwCompression", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "HwCompression", "0", ModuleScan.strFilePath);
                }
                if (this.chkFrontBackMergingEnabled.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingEnabled", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingEnabled", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingLocation", this.cboFrontBackMergingLocation.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingRotation", this.cboFrontBackMergingRotation.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingTarget", this.cboFrontBackMergingTarget.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingTargetMode", this.cboFrontBackMergingTargetMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FrontBackMergingTargetSize", double.Parse(this.txtFrontBackMergingTargetSize.Text).ToString("F3"), ModuleScan.strFilePath);
                if (this.chkDivideLongPage.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "DivideLongPage", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "DivideLongPage", "0", ModuleScan.strFilePath);
                }
                if (this.chkCharacterExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "CharacterExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "CharacterExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkReversedTypeExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "ReversedTypeExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "ReversedTypeExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkHalftoneRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "HalftoneRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "HalftoneRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStampRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "StampRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "StampRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkSimpleSlicePatternRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SimpleSlicePatternRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SimpleSlicePatternRemoval", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "FrontBackDetection", this.cboFrontBackDetection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "PaperProtection", this.cboPaperProtection.SelectedIndex.ToString(), ModuleScan.strFilePath);

                ModuleScan.WritePrivateProfileString("Scan", "ColorReproduction", this.cboColorReproduction.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ColorReproductionBrightness", this.txtColorReproductionBrightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ColorReproductionContrast", this.txtColorReproductionContrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ColorReproductionHighlight", txtColorReproductionHighlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ColorReproductionShadow", txtColorReproductionShadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ColorReproductionCustomGamma", this.txtColorReproductionCustomGamma.Text, ModuleScan.strFilePath);
                if (this.chkAdjustRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AdjustRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "AdjustRGB", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "AdjustRGBR", this.txtAdjustRGBR.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AdjustRGBG", this.txtAdjustRGBG.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AdjustRGBB", this.txtAdjustRGBB.Text, ModuleScan.strFilePath);

                if(this.MenuItemShowSourceUI.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "ShowSourceUI", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "ShowSourceUI", "0", ModuleScan.strFilePath);
                }
                if(this.MenuItemCloseSourceUI.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "CloseSourceUI", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "CloseSourceUI", "0", ModuleScan.strFilePath);
                }
                if(this.MenuItemSourceCurrentScan.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SourceCurrentScan", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SourceCurrentScan", "0", ModuleScan.strFilePath);
                }
                if(this.MenuItemSilentMode.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SilentMode", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "SilentMode", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "CarrierSheetClippingMode", ModuleScan.intCarrierSheetClippingMode.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Report", ModuleScan.intReport.ToString(), ModuleScan.strFilePath);
                if(this.MenuItemIndicator.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Indicator", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Indicator", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Highlight", txtHighlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Shadow", txtShadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "OverScan", cboOverScan.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Unit", cboUnit.SelectedIndex.ToString(), ModuleScan.strFilePath);

                if(this.chkBarcodeDetection.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeDetection", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeDetection", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "BarcodeDirection", this.cboBarcodeDirection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BarcodeRegionLeft", double.Parse(this.txtBarcodeRegionLeft.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BarcodeRegionTop", double.Parse(this.txtBarcodeRegionTop.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BarcodeRegionWidth", double.Parse(this.txtBarcodeRegionWidth.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "BarcodeRegionLength", double.Parse(this.txtBarcodeRegionLength.Text).ToString("F3"), ModuleScan.strFilePath);
                if(this.chkEAN8.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_EAN8", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_EAN8", "0", ModuleScan.strFilePath);
                }
                if(this.chkEAN13.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_EAN13", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_EAN13", "0", ModuleScan.strFilePath);
                }
                if(this.chkCode3of9.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_Code3of9", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_Code3of9", "0", ModuleScan.strFilePath);
                }
                if(this.chkCode128.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_Code128", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_Code128", "0", ModuleScan.strFilePath);
                }
                if(this.chkITF.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_ITF", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_ITF", "0", ModuleScan.strFilePath);
                }
                if(this.chkUPCA.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_UPC-A", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_UPC-A", "0", ModuleScan.strFilePath);
                }
                if(this.chkCodabar.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_Codabar", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_Codabar", "0", ModuleScan.strFilePath);
                }
                if(this.chkPDF417.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_PDF417", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_PDF417", "0", ModuleScan.strFilePath);
                }
                if(this.chkQRCode.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_QRCode", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_QRCode", "0", ModuleScan.strFilePath);
                }
                if (this.chkDataMatrix.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_DataMatrix", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeType_DataMatrix", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "BarcodeMaxSearchPriorities", this.txtBarcodeMaxSearchPriorities.Text, ModuleScan.strFilePath);
                if (this.chkBarcodeNotDetectionNotice.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeNotDetectionNotice", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "BarcodeNotDetectionNotice", "0", ModuleScan.strFilePath);
                }

                if(this.chkPatchCodeDetection.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeDetection", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeDetection", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "PatchCodeDirection", this.cboPatchCodeDirection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if(this.chkPatch1.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch1", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch1", "0", ModuleScan.strFilePath);
                }
                if(this.chkPatch2.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch2", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch2", "0", ModuleScan.strFilePath);
                }
                if(this.chkPatch3.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch3", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch3", "0", ModuleScan.strFilePath);
                }
                if(this.chkPatch4.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch4", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch4", "0", ModuleScan.strFilePath);
                }
                if(this.chkPatch6.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch6", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_Patch6", "0", ModuleScan.strFilePath);
                }
                if(this.chkPatchT.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_PatchT", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "PatchCodeType_PatchT", "0", ModuleScan.strFilePath);
                }

                ModuleScan.WritePrivateProfileString("Scan", "EdgeFiller", this.cboEdgeFiller.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EdgeFillerTop", double.Parse(this.txtEdgeFillerTop.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EdgeFillerBottom", double.Parse(this.txtEdgeFillerBottom.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EdgeFillerLeft", double.Parse(this.txtEdgeFillerLeft.Text).ToString("F3"), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "EdgeFillerRight", double.Parse(this.txtEdgeFillerRight.Text).ToString("F3"), ModuleScan.strFilePath);
                if(this.chkEdgeRepair.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "EdgeRepair", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "EdgeRepair", "0", ModuleScan.strFilePath);
                }

                ModuleScan.WritePrivateProfileString("Scan", "MultiStreamMode", this.cboMultiStreamMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "MultiStreamFileNameMode", this.cboMultiStreamFileNameMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "MultiStreamDefaultValueMode", this.cboMultiStreamDefaultValueMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1PixelType", this.cboStream1PixelType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1FileType", this.cboStream1FileType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1CompressionType", this.cboStream1CompressionType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Resolution", this.cboStream1Resolution.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1CustomResolution", this.txtStream1CustomResolution.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileCounterEx1", this.txtFileCounterEx1.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileName1", this.txtFileName1.Text, ModuleScan.strFilePath);
                if (this.chkStream1AutoBright.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1AutoBright", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1AutoBright", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream1Reverse.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1Reverse", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1Reverse", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Gamma", this.cboStream1Gamma.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1GammaFile", this.txtStream1GammaFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Brightness", this.txtStream1Brightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Contrast", this.txtStream1Contrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1CustomGamma", this.txtStream1CustomGamma.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Background", this.cboStream1Background.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Sharpness", this.cboStream1Sharpness.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Threshold", this.txtStream1Threshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1DTCSensitivity", this.txtStream1DTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1BackgroundThreshold", this.txtStream1BackgroundThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1CharacterThickness", this.txtStream1CharacterThickness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1FadingCompensation", this.txtStream1FadingCompensation.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1NoiseRejection", this.txtStream1NoiseRejection.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1PatternRemoval", this.cboStream1PatternRemoval.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if (this.chkStream1CharacterExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1CharacterExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1CharacterExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream1ReversedTypeExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1ReversedTypeExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1ReversedTypeExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream1HalftoneRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1HalftoneRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1HalftoneRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream1StampRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1StampRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1StampRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream1SimpleSlicePatternRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1SimpleSlicePatternRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1SimpleSlicePatternRemoval", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ADTCThreshold", this.txtStream1ADTCThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1SDTCSensitivity", this.txtStream1SDTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Halftone", this.cboStream1Halftone.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1HalftoneFile", this.txtStream1HalftoneFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1SEE", this.cboStream1SEE.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Filter", this.cboStream1Filter.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1FilterSaturationSensitivity", this.txtStream1FilterSaturationSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Highlight", txtStream1Highlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1Shadow", txtStream1Shadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1BackgroundSmoothing", this.cboStream1BackgroundSmoothing.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1BackgroundSmoothness", this.txtStream1BackgroundSmoothness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ColorReproduction", this.cboStream1ColorReproduction.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ColorReproductionBrightness", this.txtStream1ColorReproductionBrightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ColorReproductionContrast", this.txtStream1ColorReproductionContrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ColorReproductionHighlight", txtStream1ColorReproductionHighlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ColorReproductionShadow", txtStream1ColorReproductionShadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1ColorReproductionCustomGamma", this.txtStream1ColorReproductionCustomGamma.Text, ModuleScan.strFilePath);
                if (this.chkStream1AdjustRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1AdjustRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1AdjustRGB", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream1sRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1sRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream1sRGB", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream1AdjustRGBR", this.txtStream1AdjustRGBR.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1AdjustRGBG", this.txtStream1AdjustRGBG.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream1AdjustRGBB", this.txtStream1AdjustRGBB.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2PixelType", this.cboStream2PixelType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2FileType", this.cboStream2FileType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2CompressionType", this.cboStream2CompressionType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Resolution", this.cboStream2Resolution.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2CustomResolution", this.txtStream2CustomResolution.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileCounterEx2", this.txtFileCounterEx2.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileName2", this.txtFileName2.Text, ModuleScan.strFilePath);
                if (this.chkStream2AutoBright.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2AutoBright", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2AutoBright", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream2Reverse.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2Reverse", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2Reverse", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Gamma", this.cboStream2Gamma.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2GammaFile", this.txtStream2GammaFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Brightness", this.txtStream2Brightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Contrast", this.txtStream2Contrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2CustomGamma", this.txtStream2CustomGamma.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Background", this.cboStream2Background.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Sharpness", this.cboStream2Sharpness.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Threshold", this.txtStream2Threshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2DTCSensitivity", this.txtStream2DTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2BackgroundThreshold", this.txtStream2BackgroundThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2CharacterThickness", this.txtStream2CharacterThickness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2FadingCompensation", this.txtStream2FadingCompensation.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2NoiseRejection", this.txtStream2NoiseRejection.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2PatternRemoval", this.cboStream2PatternRemoval.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if (this.chkStream2CharacterExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2CharacterExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2CharacterExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream2ReversedTypeExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2ReversedTypeExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2ReversedTypeExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream2HalftoneRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2HalftoneRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2HalftoneRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream2StampRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2StampRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2StampRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream2SimpleSlicePatternRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2SimpleSlicePatternRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2SimpleSlicePatternRemoval", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ADTCThreshold", this.txtStream2ADTCThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2SDTCSensitivity", this.txtStream2SDTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Halftone", this.cboStream2Halftone.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2HalftoneFile", this.txtStream2HalftoneFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2SEE", this.cboStream2SEE.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Filter", this.cboStream2Filter.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2FilterSaturationSensitivity", this.txtStream2FilterSaturationSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Highlight", txtStream2Highlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2Shadow", txtStream2Shadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2BackgroundSmoothing", this.cboStream2BackgroundSmoothing.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2BackgroundSmoothness", this.txtStream2BackgroundSmoothness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ColorReproduction", this.cboStream2ColorReproduction.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ColorReproductionBrightness", this.txtStream2ColorReproductionBrightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ColorReproductionContrast", this.txtStream2ColorReproductionContrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ColorReproductionHighlight", txtStream2ColorReproductionHighlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ColorReproductionShadow", txtStream2ColorReproductionShadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2ColorReproductionCustomGamma", this.txtStream2ColorReproductionCustomGamma.Text, ModuleScan.strFilePath);
                if (this.chkStream2AdjustRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2AdjustRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2AdjustRGB", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream2sRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2sRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream2sRGB", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream2AdjustRGBR", this.txtStream2AdjustRGBR.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2AdjustRGBG", this.txtStream2AdjustRGBG.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream2AdjustRGBB", this.txtStream2AdjustRGBB.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3PixelType", this.cboStream3PixelType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3FileType", this.cboStream3FileType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3CompressionType", this.cboStream3CompressionType.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Resolution", this.cboStream3Resolution.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3CustomResolution", this.txtStream3CustomResolution.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileCounterEx3", this.txtFileCounterEx3.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "FileName3", this.txtFileName3.Text, ModuleScan.strFilePath);
                if (this.chkStream3AutoBright.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3AutoBright", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3AutoBright", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream3Reverse.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3Reverse", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3Reverse", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Gamma", this.cboStream3Gamma.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3GammaFile", this.txtStream3GammaFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Brightness", this.txtStream3Brightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Contrast", this.txtStream3Contrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3CustomGamma", this.txtStream3CustomGamma.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Background", this.cboStream3Background.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Sharpness", this.cboStream3Sharpness.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Threshold", this.txtStream3Threshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3DTCSensitivity", this.txtStream3DTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3BackgroundThreshold", this.txtStream3BackgroundThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3CharacterThickness", this.txtStream3CharacterThickness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3FadingCompensation", this.txtStream3FadingCompensation.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3NoiseRejection", this.txtStream3NoiseRejection.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3PatternRemoval", this.cboStream3PatternRemoval.SelectedIndex.ToString(), ModuleScan.strFilePath);
                if (this.chkStream3CharacterExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3CharacterExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3CharacterExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream3ReversedTypeExtraction.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3ReversedTypeExtraction", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3ReversedTypeExtraction", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream3HalftoneRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3HalftoneRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3HalftoneRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream3StampRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3StampRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3StampRemoval", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream3SimpleSlicePatternRemoval.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3SimpleSlicePatternRemoval", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3SimpleSlicePatternRemoval", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ADTCThreshold", this.txtStream3ADTCThreshold.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3SDTCSensitivity", this.txtStream3SDTCSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Halftone", this.cboStream3Halftone.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3HalftoneFile", this.txtStream3HalftoneFile.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3SEE", this.cboStream3SEE.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Filter", this.cboStream3Filter.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3FilterSaturationSensitivity", this.txtStream3FilterSaturationSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Highlight", txtStream3Highlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3Shadow", txtStream3Shadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3BackgroundSmoothing", this.cboStream3BackgroundSmoothing.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3BackgroundSmoothness", this.txtStream3BackgroundSmoothness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ColorReproduction", this.cboStream3ColorReproduction.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ColorReproductionBrightness", this.txtStream3ColorReproductionBrightness.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ColorReproductionContrast", this.txtStream3ColorReproductionContrast.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ColorReproductionHighlight", txtStream3ColorReproductionHighlight.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ColorReproductionShadow", txtStream3ColorReproductionShadow.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3ColorReproductionCustomGamma", this.txtStream3ColorReproductionCustomGamma.Text, ModuleScan.strFilePath);
                if (this.chkStream3AdjustRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3AdjustRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3AdjustRGB", "0", ModuleScan.strFilePath);
                }
                if (this.chkStream3sRGB.Checked == true)
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3sRGB", "1", ModuleScan.strFilePath);
                }
                else
                {
                    ModuleScan.WritePrivateProfileString("Scan", "Stream3sRGB", "0", ModuleScan.strFilePath);
                }
                ModuleScan.WritePrivateProfileString("Scan", "Stream3AdjustRGBR", this.txtStream3AdjustRGBR.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3AdjustRGBG", this.txtStream3AdjustRGBG.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "Stream3AdjustRGBB", this.txtStream3AdjustRGBB.Text, ModuleScan.strFilePath);

                ModuleScan.WritePrivateProfileString("Scan", "AutoProfile", formAutoProfile.cboAutoProfile.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "AutoProfileSensitivity", formAutoProfile.txtAutoProfileSensitivity.Text, ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "ManualFeedMode", this.cboManualFeedMode.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "StapleDetection", this.cboStapleDetection.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "HwAutomaticDeskew", this.cboHwAutomaticDeskew.SelectedIndex.ToString(), ModuleScan.strFilePath);
                ModuleScan.WritePrivateProfileString("Scan", "HwMoireReductionMode", this.cboHwMoireReductionMode.SelectedIndex.ToString(), ModuleScan.strFilePath);

            }
            catch (System.Exception)
            {
                MessageBox.Show("It cannot write in a file.", "fiScanTest");
            }
        }

        //----------------------------------------------------------------------------
        //   Function    : A default value is used when reading of a ini file goes wrong
        //   Argument    : Nothing
        //   Return code : Nothing
        //----------------------------------------------------------------------------
        private void DefaultSet()
        {
            this.cboScanTo.SelectedIndex = ModuleScan.TYPE_FILE;
            this.cboFileType.SelectedIndex = ModuleScan.FILE_TIF;
            this.txtFileName.Text = ModuleScan.strDirPath + "\\image########";
            this.cboOverwrite.SelectedIndex = ModuleScan.OW_CONFIRM;
            this.txtFileCounterEx.Text = "1";
            this.cboCompType.SelectedIndex = ModuleScan.COMP_MMR;
            this.cboJpegQuality.SelectedIndex = ModuleScan.COMP_JPEG4;
            this.cboScanMode.SelectedIndex = ModuleScan.SM_NORMAL;
            this.chkScanContinue.Checked = false;
            this.cboScanContinueMode.SelectedIndex = 0;
            this.cboPaperSupply.SelectedIndex = ModuleScan.ADF;
            this.txtScanCount.Text = "-1";
            this.cboPaperSize.SelectedIndex = ModuleScan.PSIZE_A4;
            this.chkLongPage.Checked = false;
            this.cboOrientation.SelectedIndex = ModuleScan.PORTRAIT;
            this.txtCustomPaperWidth.Text = "8.268";
            this.txtCustomPaperLength.Text = "11.693";
            this.txtRegionLeft.Text = "0";
            this.txtRegionTop.Text = "0";
            this.txtRegionWidth.Text = "0";
            this.txtRegionLength.Text = "0";
            this.chkUndefinedScanning.Checked = false;
            this.cboBackgroundColor.SelectedIndex = ModuleScan.NONE;
            this.cboPixelType.SelectedIndex = ModuleScan.PIXEL_BLACK_WHITE;
            this.txtAutomaticColorSensitivity.Text = "0";
            this.cboAutomaticColorBackground.SelectedIndex = 0;
            this.cboResolution.SelectedIndex = ModuleScan.RS_300;
            this.txtCustomResolution.Text = "300";
            this.txtBrightness.Text = "128";
            this.txtContrast.Text = "128";
            this.txtThreshold.Text = "128";
            this.chkReverse.Checked = false;
            this.chkMirroring.Checked = false;
            this.cboRotation.SelectedIndex = ModuleScan.RT_NONE;
            this.cboAutomaticRotateMode.SelectedIndex = ModuleScan.ARM_STANDARD;
            this.cboBackground.SelectedIndex = ModuleScan.MODE_OFF;
            this.cboOutline.SelectedIndex = ModuleScan.NONE;
            this.cboHalftone.SelectedIndex = ModuleScan.NONE;
            this.txtHalftoneFile.Text = "";
            this.cboGamma.SelectedIndex = ModuleScan.NONE;
            this.txtGammaFile.Text = "";
            this.txtCustomGamma.Text = "2.2";
            this.cboAutoSeparation.SelectedIndex = ModuleScan.AS_OFF;
            this.cboSEE.SelectedIndex = ModuleScan.SEE_OFF;
            this.chkVerticalLineReduction.Checked = false;
            this.cboThresholdCurve.SelectedIndex = ModuleScan.TH_CURVE1;
            this.cboNoiseRemoval.SelectedIndex = 0;
            this.chkPreFiltering.Checked = false;
            this.chkSmoothing.Checked = false;
            this.chkAIQCNotice.Checked = false;
            this.chkMultiFeedNotice.Checked = false;
            this.txtBackgroundSmoothness.Text = "5";
            this.cboCropPriority.SelectedIndex = 0;
            this.cboDeskew.SelectedIndex = 2;
            this.cboDeskewBackground.SelectedIndex = 1;
            this.cboDeskewMode.SelectedIndex = 1;
            this.cboBlankPageSkipMode.SelectedIndex = 0;
            this.cboBlankPageSkipTabPage.SelectedIndex = 0;
            this.txtBlankPageIgnoreAreaSize.Text = "16";
            this.txtCropMarginSize.Text = "0";
            this.cboSelectOutputSize.SelectedIndex = ModuleScan.SOS_MARGIN;
            this.txtMultiFeedModeChangeSize.Text = "0";
            this.cboLengthDetection.SelectedIndex = 0;
            this.chkEndorser.Checked = false;
            this.cboEndorserDialog.SelectedIndex = ModuleScan.EDD_OFF;
            this.txtEndorserCounter.Text = "0";
            this.txtEndorserString.Text = "";
            this.txtEndorserOffset.Text = "0";
            this.cboEndorserDirection.SelectedIndex = 1;
            this.cboEndorserCountStep.SelectedIndex = 1;
            this.cboEndorserCountDirection.SelectedIndex = 0;
            this.cboEndorserFont.SelectedIndex = ModuleScan.EDF_HORIZONTAL;
            this.chkSynchronizationDigitalEndorser.Checked = false;
            this.chkEndorser.Enabled = true;
            this.cboEndorserDialog.Enabled = false;
            this.txtEndorserCounter.Enabled = false;
            this.txtEndorserString.Enabled = false;
            this.txtEndorserOffset.Enabled = false;
            this.cboEndorserDirection.Enabled = false;
            this.cboEndorserCountStep.Enabled = false;
            this.cboEndorserCountDirection.Enabled = false;
            this.cboEndorserFont.Enabled = false;
            this.chkSynchronizationDigitalEndorser.Enabled = false;
            this.chkDigitalEndorser.Checked = false;
            this.txtDigitalEndorserCounter.Text = "0";
            this.txtDigitalEndorserString.Text = "";
            this.txtDigitalEndorserXOffset.Text = "0";
            this.txtDigitalEndorserYOffset.Text = "0";
            this.cboDigitalEndorserDirection.SelectedIndex = 0;
            this.cboDigitalEndorserCountStep.SelectedIndex = 1;
            this.cboDigitalEndorserCountDirection.SelectedIndex = 0;
            this.chkDigitalEndorser.Enabled = true;
            this.txtDigitalEndorserCounter.Enabled = false;
            this.txtDigitalEndorserString.Enabled = false;
            this.txtDigitalEndorserXOffset.Enabled = false;
            this.txtDigitalEndorserYOffset.Enabled = false;
            this.cboDigitalEndorserDirection.Enabled = false;
            this.cboDigitalEndorserCountStep.Enabled = false;
            this.cboDigitalEndorserCountDirection.Enabled = false;
            this.cboJobControl.SelectedIndex = ModuleScan.NONE;
            this.cboBinding.SelectedIndex = 0;
            this.cboMultiFeed.SelectedIndex = 0;
            this.cboFilter.SelectedIndex = ModuleScan.FILTER_GREEN;
            this.txtFilterSaturationSensitivity.Text = "50";
            this.txtSkipWhitePage.Text = "0";
            this.txtSkipBlackPage.Text = "0";
            this.chkAutoBorderDetection.Checked = false;
            this.cboBlankPageNotice.SelectedIndex = 0;
            this.chkHwCompression.Checked = false;
            this.chkFrontBackMergingEnabled.Checked = false;
            this.cboFrontBackMergingLocation.SelectedIndex = ModuleScan.FBML_RIGHT;
            this.cboFrontBackMergingRotation.SelectedIndex = ModuleScan.FBMR_NONE;
            this.cboFrontBackMergingTarget.SelectedIndex = ModuleScan.FBMT_ALL;
            this.cboFrontBackMergingTargetMode.SelectedIndex = ModuleScan.FBMTM_INDEX_CUSTOM;
            this.chkDivideLongPage.Checked = false;
            this.chkAutomaticSenseMedium.Checked = false;
            this.cboBackgroundSmoothing.SelectedIndex = 0;
            this.chkAutoBright.Checked = false;
            this.cboJobControlMode.SelectedIndex = ModuleScan.JCM_SPECIAL_DOCUMENT;
            this.txtBlankPageSkip.Text = "0";
            this.txtDTCSensitivity.Text = "50";
            this.txtBackgroundThreshold.Text = "50";
            this.txtCharacterThickness.Text = "5";
            this.txtSDTCSensitivity.Text = "2";
            this.txtNoiseRejection.Text = "0";
            this.txtADTCThreshold.Text = "83";
            this.cboSharpness.SelectedIndex = ModuleScan.SH_NONE;
            this.txtFadingCompensation.Text = "0";
            this.chksRGB.Checked = false;
            this.cboPunchHoleRemoval.SelectedIndex = ModuleScan.PHR_DO_NOT_REMOVE;
            this.cboPunchHoleRemovalMode.SelectedIndex = ModuleScan.PHRM_STANDARD;
            this.cboPatternRemoval.SelectedIndex = 1;
            this.chkCharacterExtraction.Checked = false;
            this.chkReversedTypeExtraction.Checked = true;
            this.chkHalftoneRemoval.Checked = true;
            this.chkStampRemoval.Checked = true;
            this.chkSimpleSlicePatternRemoval.Checked = false;
            this.cboFrontBackDetection.SelectedIndex = ModuleScan.FBD_NONE;
            this.cboPaperProtection.SelectedIndex = ModuleScan.PP_DRIVERSETTING;
            this.cboColorReproduction.SelectedIndex = ModuleScan.CR_CONTRAST;
            this.txtColorReproductionBrightness.Text = "128";
            this.txtColorReproductionContrast.Text = "128";
            this.txtColorReproductionHighlight.Text = "255";
            this.txtColorReproductionShadow.Text = "0";
            this.txtColorReproductionCustomGamma.Text = "1.0";
            this.chkAdjustRGB.Checked = false;
            this.txtAdjustRGBR.Text = "128";
            this.txtAdjustRGBG.Text = "128";
            this.txtAdjustRGBB.Text = "128";
            this.chkBarcodeDetection.Checked = false;
            this.cboBarcodeDirection.SelectedIndex = ModuleScan.BD_HORIZONTAL_VERTICAL;
            this.txtBarcodeRegionLeft.Text = "0";
            this.txtBarcodeRegionTop.Text = "0";
            this.txtBarcodeRegionWidth.Text = "0";
            this.txtBarcodeRegionLength.Text = "0";
            this.chkEAN8.Checked = true;
            this.chkEAN13.Checked = true;
            this.chkCode3of9.Checked = true;
            this.chkCode128.Checked = true;
            this.chkITF.Checked = true;
            this.chkUPCA.Checked = true;
            this.chkCodabar.Checked = true;
            this.chkPDF417.Checked = true;
            this.chkQRCode.Checked = true;
            this.chkDataMatrix.Checked = false;
            this.txtBarcodeMaxSearchPriorities.Text = "1";
            this.chkBarcodeNotDetectionNotice.Checked = false;
            this.cboBarcodeDirection.Enabled = false;
            this.txtBarcodeRegionLeft.Enabled = false;
            this.txtBarcodeRegionTop.Enabled = false;
            this.txtBarcodeRegionWidth.Enabled = false;
            this.txtBarcodeRegionLength.Enabled = false;
            this.chkEAN8.Enabled = false;
            this.chkEAN13.Enabled = false;
            this.chkCode3of9.Enabled = false;
            this.chkCode128.Enabled = false;
            this.chkITF.Enabled = false;
            this.chkUPCA.Enabled = false;
            this.chkCodabar.Enabled = false;
            this.chkPDF417.Enabled = false;
            this.chkQRCode.Enabled = false;
            this.chkDataMatrix.Enabled = false;
            this.txtBarcodeMaxSearchPriorities.Enabled = false;
            this.chkBarcodeNotDetectionNotice.Enabled = false;
            this.chkPatchCodeDetection.Checked = false;
            this.cboPatchCodeDirection.SelectedIndex = ModuleScan.PD_VERTICAL;
            this.chkPatch1.Checked = true;
            this.chkPatch2.Checked = true;
            this.chkPatch3.Checked = true;
            this.chkPatch4.Checked = true;
            this.chkPatch6.Checked = true;
            this.chkPatchT.Checked = true;
            this.cboEdgeFiller.SelectedIndex = ModuleScan.EF_OFF;
            this.txtEdgeFillerTop.Text = "0";
            this.txtEdgeFillerBottom.Text = "0";
            this.txtEdgeFillerLeft.Text = "0";
            this.txtEdgeFillerRight.Text = "0";
            this.chkEdgeRepair.Checked = false;

            this.MenuItemShowSourceUI.Checked = false;
            this.MenuItemCloseSourceUI.Checked = false;
            this.MenuItemSourceCurrentScan.Checked = false;
            this.MenuItemTWAINTemplate.Enabled = this.MenuItemSourceCurrentScan.Checked;
            this.MenuItemSilentMode.Checked = false;
            ModuleScan.intCarrierSheetClippingMode = ModuleScan.CSCM_DRIVERSETTING;
            this.MenuItemCarrierSheetClippingModeContent.Checked = false;
            this.MenuItemCarrierSheetClippingModeEdge.Checked = false;
            this.MenuItemCarrierSheetClippingModeDriverSetting.Checked = true;
            ModuleScan.intReport = ModuleScan.NONE;
            this.MenuItemReportOFF.Checked = true;
            this.MenuItemReportDisplay.Checked = false;
            this.MenuItemReportFile.Checked = false;
            this.MenuItemReportBoth.Checked = false;
            this.MenuItemIndicator.Checked = true;
            this.MenuItemShowEvent.Checked = false;
            this.MenuItemOutputResult.Checked = false;

            if (Int32.Parse(this.txtShadow.Text) >= 230)
            {
                this.txtShadow.Text = "10";
                this.txtHighlight.Text = "230";
            }
            else
            {
                this.txtHighlight.Text = "230";
                this.txtShadow.Text = "10";
            }
            this.txtHighlight.Enabled = false;
            this.txtShadow.Enabled = false;
            this.cboOverScan.SelectedIndex = ModuleScan.OVERSCAN_OFF;
            this.cboUnit.SelectedIndex = ModuleScan.UNIT_INCHES;
            this.txtFrontBackMergingTargetSize.Text = "1";

            this.cboMultiStreamMode.SelectedIndex = ModuleScan.MSM_OFF;
            this.cboMultiStreamFileNameMode.SelectedIndex = ModuleScan.MSFNM_OFF;
            this.cboMultiStreamDefaultValueMode.SelectedIndex = ModuleScan.MSDVM_OFF;
            this.cboStream1PixelType.SelectedIndex = ModuleScan.PIXEL_BLACK_WHITE;
            this.cboStream1FileType.SelectedIndex = ModuleScan.FILE_TIF;
            this.cboStream1CompressionType.SelectedIndex = ModuleScan.COMP_MMR;
            this.cboStream1Resolution.SelectedIndex = ModuleScan.RS_300;
            this.txtStream1CustomResolution.Text = "300";
            this.txtFileCounterEx1.Text = "1";
            this.txtFileName1.Text = ModuleScan.strDirPath + "\\image1_########";
            this.chkStream1AutoBright.Checked = false;
            this.chkStream1Reverse.Checked = false;
            this.cboStream1Gamma.SelectedIndex = ModuleScan.NONE;
            this.txtStream1GammaFile.Text = "";
            this.txtStream1Brightness.Text = "128";
            this.txtStream1Contrast.Text = "128";
            this.txtStream1CustomGamma.Text = "2.2";
            this.cboStream1Background.SelectedIndex = ModuleScan.MODE_OFF;
            this.cboStream1Sharpness.SelectedIndex = ModuleScan.SH_NONE;
            this.txtStream1Threshold.Text = "128";
            this.txtStream1DTCSensitivity.Text = "50";
            this.txtStream1BackgroundThreshold.Text = "50";
            this.txtStream1CharacterThickness.Text = "5";
            this.txtStream1FadingCompensation.Text = "0";
            this.txtStream1NoiseRejection.Text = "0";
            this.cboStream1PatternRemoval.SelectedIndex = 1;
            this.chkStream1CharacterExtraction.Checked = false;
            this.chkStream1ReversedTypeExtraction.Checked = true;
            this.chkStream1HalftoneRemoval.Checked = true;
            this.chkStream1StampRemoval.Checked = true;
            this.chkStream1SimpleSlicePatternRemoval.Checked = false;
            this.txtStream1ADTCThreshold.Text = "83";
            this.txtStream1SDTCSensitivity.Text = "2";
            this.cboStream1Halftone.SelectedIndex = ModuleScan.NONE;
            this.txtStream1HalftoneFile.Text = "";
            this.cboStream1SEE.SelectedIndex = ModuleScan.SEE_OFF;
            this.cboStream1Filter.SelectedIndex = ModuleScan.FILTER_GREEN;
            this.txtStream1FilterSaturationSensitivity.Text = "50";
            if (Int32.Parse(this.txtStream1Shadow.Text) >= 230)
            {
                this.txtStream1Shadow.Text = "10";
                this.txtStream1Highlight.Text = "230";
            }
            else
            {
                this.txtStream1Highlight.Text = "230";
                this.txtStream1Shadow.Text = "10";
            }
            this.cboStream1BackgroundSmoothing.SelectedIndex = 0;
            this.txtStream1BackgroundSmoothness.Text = "5";
            this.cboStream1ColorReproduction.SelectedIndex = ModuleScan.CR_CONTRAST;
            this.txtStream1ColorReproductionBrightness.Text = "128";
            this.txtStream1ColorReproductionContrast.Text = "128";
            this.txtStream1ColorReproductionHighlight.Text = "255";
            this.txtStream1ColorReproductionShadow.Text = "0";
            this.txtStream1ColorReproductionCustomGamma.Text = "1.0";
            this.chkStream1AdjustRGB.Checked = false;
            this.chkStream1sRGB.Checked = false;
            this.txtStream1AdjustRGBR.Text = "128";
            this.txtStream1AdjustRGBG.Text = "128";
            this.txtStream1AdjustRGBB.Text = "128";
            this.cboStream2PixelType.SelectedIndex = ModuleScan.PIXEL_BLACK_WHITE;
            this.cboStream2FileType.SelectedIndex = ModuleScan.FILE_TIF;
            this.cboStream2CompressionType.SelectedIndex = ModuleScan.COMP_MMR;
            this.cboStream2Resolution.SelectedIndex = ModuleScan.RS_300;
            this.txtStream2CustomResolution.Text = "300";
            this.txtFileCounterEx2.Text = "1";
            this.txtFileName2.Text = ModuleScan.strDirPath + "\\image2_########";
            this.chkStream2AutoBright.Checked = false;
            this.chkStream2Reverse.Checked = false;
            this.cboStream2Gamma.SelectedIndex = ModuleScan.NONE;
            this.txtStream2GammaFile.Text = "";
            this.txtStream2Brightness.Text = "128";
            this.txtStream2Contrast.Text = "128";
            this.txtStream2CustomGamma.Text = "2.2";
            this.cboStream2Background.SelectedIndex = ModuleScan.MODE_OFF;
            this.cboStream2Sharpness.SelectedIndex = ModuleScan.SH_NONE;
            this.txtStream2Threshold.Text = "128";
            this.txtStream2DTCSensitivity.Text = "50";
            this.txtStream2BackgroundThreshold.Text = "50";
            this.txtStream2CharacterThickness.Text = "5";
            this.txtStream2FadingCompensation.Text = "0";
            this.txtStream2NoiseRejection.Text = "0";
            this.cboStream2PatternRemoval.SelectedIndex = 1;
            this.chkStream2CharacterExtraction.Checked = false;
            this.chkStream2ReversedTypeExtraction.Checked = true;
            this.chkStream2HalftoneRemoval.Checked = true;
            this.chkStream2StampRemoval.Checked = true;
            this.chkStream2SimpleSlicePatternRemoval.Checked = false;
            this.txtStream2ADTCThreshold.Text = "83";
            this.txtStream2SDTCSensitivity.Text = "2";
            this.cboStream2Halftone.SelectedIndex = ModuleScan.NONE;
            this.txtStream2HalftoneFile.Text = "";
            this.cboStream2SEE.SelectedIndex = ModuleScan.SEE_OFF;
            this.cboStream2Filter.SelectedIndex = ModuleScan.FILTER_GREEN;
            this.txtStream2FilterSaturationSensitivity.Text = "50";
            if (Int32.Parse(this.txtStream2Shadow.Text) >= 230)
            {
                this.txtStream2Shadow.Text = "10";
                this.txtStream2Highlight.Text = "230";
            }
            else
            {
                this.txtStream2Highlight.Text = "230";
                this.txtStream2Shadow.Text = "10";
            }
            this.cboStream2BackgroundSmoothing.SelectedIndex = 0;
            this.txtStream2BackgroundSmoothness.Text = "5";
            this.cboStream2ColorReproduction.SelectedIndex = ModuleScan.CR_CONTRAST;
            this.txtStream2ColorReproductionBrightness.Text = "128";
            this.txtStream2ColorReproductionContrast.Text = "128";
            this.txtStream2ColorReproductionHighlight.Text = "255";
            this.txtStream2ColorReproductionShadow.Text = "0";
            this.txtStream2ColorReproductionCustomGamma.Text = "1.0";
            this.chkStream2AdjustRGB.Checked = false;
            this.chkStream2sRGB.Checked = false;
            this.txtStream2AdjustRGBR.Text = "128";
            this.txtStream2AdjustRGBG.Text = "128";
            this.txtStream2AdjustRGBB.Text = "128";
            this.cboStream3PixelType.SelectedIndex = ModuleScan.PIXEL_BLACK_WHITE;
            this.cboStream3FileType.SelectedIndex = ModuleScan.FILE_TIF;
            this.cboStream3CompressionType.SelectedIndex = ModuleScan.COMP_MMR;
            this.cboStream3Resolution.SelectedIndex = ModuleScan.RS_300;
            this.txtStream3CustomResolution.Text = "300";
            this.txtFileCounterEx3.Text = "1";
            this.txtFileName3.Text = ModuleScan.strDirPath + "\\image3_########";
            this.chkStream3AutoBright.Checked = false;
            this.chkStream3Reverse.Checked = false;
            this.cboStream3Gamma.SelectedIndex = ModuleScan.NONE;
            this.txtStream3GammaFile.Text = "";
            this.txtStream3Brightness.Text = "128";
            this.txtStream3Contrast.Text = "128";
            this.txtStream3CustomGamma.Text = "2.2";
            this.cboStream3Background.SelectedIndex = ModuleScan.MODE_OFF;
            this.cboStream3Sharpness.SelectedIndex = ModuleScan.SH_NONE;
            this.txtStream3Threshold.Text = "128";
            this.txtStream3DTCSensitivity.Text = "50";
            this.txtStream3BackgroundThreshold.Text = "50";
            this.txtStream3CharacterThickness.Text = "5";
            this.txtStream3FadingCompensation.Text = "0";
            this.txtStream3NoiseRejection.Text = "0";
            this.cboStream3PatternRemoval.SelectedIndex = 1;
            this.chkStream3CharacterExtraction.Checked = false;
            this.chkStream3ReversedTypeExtraction.Checked = true;
            this.chkStream3HalftoneRemoval.Checked = true;
            this.chkStream3StampRemoval.Checked = true;
            this.chkStream3SimpleSlicePatternRemoval.Checked = false;
            this.txtStream3ADTCThreshold.Text = "83";
            this.txtStream3SDTCSensitivity.Text = "2";
            this.cboStream3Halftone.SelectedIndex = ModuleScan.NONE;
            this.txtStream3HalftoneFile.Text = "";
            this.cboStream3SEE.SelectedIndex = ModuleScan.SEE_OFF;
            this.cboStream3Filter.SelectedIndex = ModuleScan.FILTER_GREEN;
            this.txtStream3FilterSaturationSensitivity.Text = "50";
            if (Int32.Parse(this.txtStream3Shadow.Text) >= 230)
            {
                this.txtStream3Shadow.Text = "10";
                this.txtStream3Highlight.Text = "230";
            }
            else
            {
                this.txtStream3Highlight.Text = "230";
                this.txtStream3Shadow.Text = "10";
            }
            this.cboStream3BackgroundSmoothing.SelectedIndex = 0;
            this.txtStream3BackgroundSmoothness.Text = "5";
            this.cboStream3ColorReproduction.SelectedIndex = ModuleScan.CR_CONTRAST;
            this.txtStream3ColorReproductionBrightness.Text = "128";
            this.txtStream3ColorReproductionContrast.Text = "128";
            this.txtStream3ColorReproductionHighlight.Text = "255";
            this.txtStream3ColorReproductionShadow.Text = "0";
            this.txtStream3ColorReproductionCustomGamma.Text = "1.0";
            this.chkStream3AdjustRGB.Checked = false;
            this.chkStream3sRGB.Checked = false;
            this.txtStream3AdjustRGBR.Text = "128";
            this.txtStream3AdjustRGBG.Text = "128";
            this.txtStream3AdjustRGBB.Text = "128";

            formAutoProfile.cboAutoProfile.SelectedIndex = ModuleScan.AP_DISABLED;
            formAutoProfile.txtAutoProfileSensitivity.Text = "3";
            this.cboManualFeedMode.SelectedIndex = ModuleScan.MFM_HARDWARESETTING;
            this.cboStapleDetection.SelectedIndex = ModuleScan.SD_ON;
            this.cboHwAutomaticDeskew.SelectedIndex = ModuleScan.HAMD_ON;
            this.cboHwMoireReductionMode.SelectedIndex = ModuleScan.HMRM_DRIVERSETTING;
        }
