﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public partial class LayerPropertiesForm : Form
{
    Mainframe SourceForm;
    byte CurrentLayer;
    Layer DataSource;
    int[] newrectangle;
    public LayerPropertiesForm(Mainframe parent, byte Layer)
    {
        SourceForm = parent;
        CurrentLayer = Layer;
            InitializeComponent();
        }
        private void LayerPropertiesForm_Load(object sender, EventArgs e)
        {
            if (SourceForm.TextureTypes.Count == 0) { TextureMode.Parent = groupBox3; TextureMode.Location = new Point(TileHeight.Location.X, LimitVisibleRegion.Location.Y); ClientSize = new Size(ClientSize.Width, ClientSize.Height - (groupBox4.Bottom - groupBox3.Bottom)); }
            else if (SourceForm.TextureTypes.Count == 1) {
                TextureModeSelect.Visible = false;
                foreach (Control foo in new Control[] { Param1, Param2, Param3, RedLabel, GreenLabel, BlueLabel, Stars, ColorBox, ColorLabel})
                { foo.Location = new Point(foo.Location.X, foo.Location.Y - TextureModeSelect.Height); }
                /*Param1.Location = new Point(Param1.Location.X, Param1.Location.Y - TextureModeSelect.Height);
                Param2.Location = new Point(Param2.Location.X, Param2.Location.Y - TextureModeSelect.Height);
                Param3.Location = new Point(Param3.Location.X, Param3.Location.Y - TextureModeSelect.Height);
                RedLabel.Location = new Point(RedLabel.Location.X, RedLabel.Location.Y - TextureModeSelect.Height);
                GreenLabel.Location = new Point(GreenLabel.Location.X, GreenLabel.Location.Y - TextureModeSelect.Height);
                BlueLabel.Location = new Point(BlueLabel.Location.X, BlueLabel.Location.Y - TextureModeSelect.Height);
                Stars.Location = new Point(Stars.Location.X, Stars.Location.Y - TextureModeSelect.Height);
                ColorBox.Location = new Point(ColorBox.Location.X, ColorBox.Location.Y - TextureModeSelect.Height);
                ColorLabel.Location = new Point(ColorLabel.Location.X, ColorLabel.Location.Y - TextureModeSelect.Height);*/
                if (SourceForm.TextureTypes[0][1] == "+") {
                    groupBox4.Height -= TextureModeSelect.Height + (Param3.Location.Y - Param1.Location.Y);
                    Height -= TextureModeSelect.Height + (Param3.Location.Y - Param1.Location.Y);
                }
                else
                {
                    groupBox4.Height -= TextureModeSelect.Height;
                    Height -= TextureModeSelect.Height;
                }
            }
            TextureModeSelect.Items.Clear();
            for (ushort i = 0; i < SourceForm.TextureTypes.Count; i++) TextureModeSelect.Items.Add(SourceForm.TextureTypes[i][0].Trim());
            LayerSelect.SelectedIndex = CurrentLayer;
        }

        private void ReadLayer(byte Layer)
        {
            DataSource = SourceForm.J2L.Layers[Layer];
            WidthBox.Value = DataSource.Width;
            HeightBox.Value = DataSource.Height;
            //PHASEWidthBox.Text = DataSource.Width.ToString();
            //PHASEHeightBox.Text = DataSource.Height.ToString();
            TileWidth.Checked = DataSource.TileWidth;
            TileHeight.Checked = DataSource.TileHeight;
            XSpeed.Text = DataSource.XSpeed.ToString();
            AutoXSpeed.Text = DataSource.AutoXSpeed.ToString();
            YSpeed.Text = DataSource.YSpeed.ToString();
            AutoYSpeed.Text = DataSource.AutoYSpeed.ToString();
            LimitVisibleRegion.Checked = DataSource.LimitVisibleRegion;
            TextureMode.Checked = DataSource.IsTextured;
            Stars.Checked = DataSource.HasStars;
            Param1.Value = DataSource.TexturParam1;
            Param2.Value = DataSource.TexturParam2;
            Param3.Value = DataSource.TexturParam3;
            //PHASEParam1.Text = DataSource.TexturParam1.ToString();
            //PHASEParam2.Text = DataSource.TexturParam2.ToString();
            //PHASEParam3.Text = DataSource.TexturParam3.ToString();
            ColorBox.BackColor = Color.FromArgb(DataSource.TexturParam1, DataSource.TexturParam2, DataSource.TexturParam3);
            TextureModeSelect.SelectedIndex = DataSource.TextureMode;
        }

        private void ApplyChanges()
        {
            newrectangle = null;
            lock (DataSource)
            {
                if (DataSource.Width != WidthBox.Value || DataSource.Height != HeightBox.Value)
                {
                    newrectangle = LayerAlign.Show(CurrentLayer, (int)WidthBox.Value - (int)DataSource.Width, (int)HeightBox.Value - (int)DataSource.Height);
                    if (newrectangle == null) return;
                }
                DataSource.Width = (uint)WidthBox.Value;
                DataSource.Height = (uint)HeightBox.Value;
                if (TileWidth.Checked) switch (DataSource.Width %4)
                {
                    case 0: DataSource.RealWidth = DataSource.Width; break;
                    case 2: DataSource.RealWidth = DataSource.Width*2; break;
                    default: DataSource.RealWidth = DataSource.Width*4; break;
                }
                else DataSource.RealWidth = DataSource.Width;
                #region Old Size Change code
                //if (newrectangle != null || (DataSource.TileWidth == false && TileWidth.Checked == true && DataSource.Width % 4 > 0))
                //{
                //    ushort[,] newTileMap = new ushort[DataSource.RealWidth, DataSource.Height];
                //    for (ushort x = 0; x < DataSource.Width; x++) for (ushort y = 0; y < DataSource.Height; y++)
                //        {
                //            newTileMap[x, y] = (
                //                x >= -newrectangle[2] &&
                //                newrectangle[2] + x < DataSource.TileMap.GetLength(0) &&
                //                y >= -newrectangle[0] &&
                //                newrectangle[0] + y < DataSource.TileMap.GetLength(1)
                //                )
                //                ? DataSource.TileMap[newrectangle[2] + x, newrectangle[0] + y]
                //                : (ushort)0;
                //        }
                //    if (CurrentLayer == 3)
                //    {
                //        uint[,] newEventMap = new uint[DataSource.Width, DataSource.Height];
                //        for (ushort x = 0; x < DataSource.Width; x++) for (ushort y = 0; y < DataSource.Height; y++)
                //            {
                //                newEventMap[x, y] = (
                //                    x >= -newrectangle[2] &&
                //                    newrectangle[2] + x < DataSource.TileMap.GetLength(0) &&
                //                    y >= -newrectangle[0] &&
                //                    newrectangle[0] + y < DataSource.TileMap.GetLength(1)
                //                    )
                //                    ? SourceForm.J2L.EventMap[newrectangle[2] + x, newrectangle[0] + y]
                //                    : (ushort)0;
                //            }
                //        SourceForm.J2L.EventMap = newEventMap;
                //    }
                //    if (TileWidth.Checked == true && DataSource.Width % 4 > 0) for (ushort x = 0; x < DataSource.Width; x++) for (ushort y = 0; y < DataSource.Height; y++)
                //            {
                //                switch (DataSource.Width % 4)
                //                {
                //                    case 2:
                //                        newTileMap[x + DataSource.Width, y] = newTileMap[x, y];
                //                        break;
                //                    default:
                //                        newTileMap[x + DataSource.Width, y] = newTileMap[x + DataSource.Width * 2, y] = newTileMap[x + DataSource.Width * 3, y] = newTileMap[x, y];
                //                        break;
                //                }
                //            }
                //    DataSource.TileMap = newTileMap;
                //} 
                #endregion
                if (newrectangle != null)
                {
                    ushort[,] newTileMap = new ushort[DataSource.Width, DataSource.Height];
                    for (ushort x = 0; x < DataSource.Width; x++) for (ushort y = 0; y < DataSource.Height; y++)
                        {
                            newTileMap[x, y] = (
                                x >= -newrectangle[2] &&
                                newrectangle[2] + x < DataSource.TileMap.GetLength(0) &&
                                y >= -newrectangle[0] &&
                                newrectangle[0] + y < DataSource.TileMap.GetLength(1)
                                )
                                ? DataSource.TileMap[newrectangle[2] + x, newrectangle[0] + y]
                                : (ushort)0;
                        }
                    if (CurrentLayer == 3)
                    {
                        AGAEvent[,] newAGAMap = null;
                        if (SourceForm.J2L.VersionType == Version.AGA) newAGAMap = new AGAEvent[DataSource.Width, DataSource.Height];
                        uint[,] newEventMap = new uint[DataSource.Width, DataSource.Height];
                        for (ushort x = 0; x < DataSource.Width; x++) for (ushort y = 0; y < DataSource.Height; y++)
                            {
                                newEventMap[x, y] = (
                                    x >= -newrectangle[2] &&
                                    newrectangle[2] + x < DataSource.TileMap.GetLength(0) &&
                                    y >= -newrectangle[0] &&
                                    newrectangle[0] + y < DataSource.TileMap.GetLength(1)
                                    )
                                    ? SourceForm.J2L.EventMap[newrectangle[2] + x, newrectangle[0] + y]
                                    : (ushort)0;
                                if (SourceForm.J2L.VersionType == Version.AGA) newAGAMap[x, y] = (
                                    x >= -newrectangle[2] &&
                                    newrectangle[2] + x < DataSource.TileMap.GetLength(0) &&
                                    y >= -newrectangle[0] &&
                                    newrectangle[0] + y < DataSource.TileMap.GetLength(1)
                                    )
                                    ? SourceForm.J2L.AGA_EventMap[newrectangle[2] + x, newrectangle[0] + y]
                                    : new AGAEvent();
                            }
                        SourceForm.J2L.EventMap = newEventMap;
                    }
                    DataSource.TileMap = newTileMap;
                }
                DataSource.TileWidth = TileWidth.Checked;
                DataSource.TileHeight = TileHeight.Checked;
                try { DataSource.XSpeed = Convert.ToSingle(XSpeed.Text); } catch { DataSource.XSpeed = 0; }
                try { DataSource.YSpeed = Convert.ToSingle(YSpeed.Text); } catch { DataSource.YSpeed = 0; }
                try { DataSource.AutoXSpeed = Convert.ToSingle(AutoXSpeed.Text); } catch { DataSource.AutoXSpeed = 0; }
                try { DataSource.AutoYSpeed = Convert.ToSingle(AutoYSpeed.Text); } catch { DataSource.AutoYSpeed = 0; }
                DataSource.LimitVisibleRegion = LimitVisibleRegion.Checked;
                DataSource.IsTextured = TextureMode.Checked;
                DataSource.HasStars = Stars.Checked;
                DataSource.TexturParam1 = (byte)Param1.Value;
                DataSource.TexturParam2 = (byte)Param2.Value;
                DataSource.TexturParam3 = (byte)Param3.Value;
                DataSource.TextureMode = (byte)TextureModeSelect.SelectedIndex;
                SourceForm.LevelHasBeenModified = true;
            }
        }

        private void TextureModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] properties = SourceForm.TextureTypes[TextureModeSelect.SelectedIndex];
            if (properties.Length >= 3)
            {
                Stars.Text = properties[2].Trim();
                Stars.Visible = true;
            } else { Stars.Visible = false; }

            Param1.Visible = Param2.Visible = Param3.Visible = RedLabel.Visible = GreenLabel.Visible = BlueLabel.Visible = false;
            if (properties.Length > 1 && properties[1] == "+")
            {
                ColorLabel.Text = properties[3].Trim();
                ColorLabel.Visible = ColorBox.Visible = true;
            }
            else
            {
                if (properties.Length >= 4) { Param1.Visible = RedLabel.Visible = true; RedLabel.Text = properties[3].Trim(); }
                if (properties.Length >= 5) { Param2.Visible = GreenLabel.Visible = true; GreenLabel.Text = properties[4].Trim(); }
                if (properties.Length >= 6) { Param3.Visible = BlueLabel.Visible = true; BlueLabel.Text = properties[5].Trim(); }
                ColorLabel.Visible = ColorBox.Visible = false;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK) ColorBox.BackColor = colorDialog1.Color;
            Param1.Value = colorDialog1.Color.R;
            Param2.Value = colorDialog1.Color.G;
            Param3.Value = colorDialog1.Color.B;
        }

        private void CancelButton_Click(object sender, EventArgs e) { Close(); }

        private void LayerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentLayer = (byte)LayerSelect.SelectedIndex;
            XSpeed.Enabled = YSpeed.Enabled = AutoXSpeed.Enabled = AutoYSpeed.Enabled = XLabel.Enabled = YLabel.Enabled = AutoXLabel.Enabled = AutoYLabel.Enabled = TextureMode.Enabled = TileWidth.Enabled = TileHeight.Enabled = (CurrentLayer != 3);
            Copy4.Enabled = (CurrentLayer != 3 && CurrentLayer != 7);
            ReadLayer(CurrentLayer);
        }

        private void TextureMode_CheckedChanged(object sender, EventArgs e)
        {
            TextureModeSelect.Enabled = Stars.Enabled = ColorBox.Enabled = ColorLabel.Enabled = Param1.Enabled = Param2.Enabled = Param3.Enabled = RedLabel.Enabled = GreenLabel.Enabled = BlueLabel.Enabled = TextureMode.Checked;
            WidthBox.Enabled = HeightBox.Enabled = WidthLabel.Enabled = HeightLabel.Enabled = !TextureMode.Checked;
            if (TextureMode.Checked) WidthBox.Value = HeightBox.Value = 8;
            else { WidthBox.Value = DataSource.Width; HeightBox.Value = DataSource.Height; }
            LimitVisibleRegion.Enabled = !(TextureMode.Checked || TileHeight.Checked);
        }
        private void TileHeight_CheckedChanged(object sender, EventArgs e) { LimitVisibleRegion.Enabled = !(TextureMode.Checked || TileHeight.Checked); }

        private void OKButton_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            Close();
        }

        private void Copy4_Click(object sender, EventArgs e)
        {
            ReadLayer(3);
            DataSource = SourceForm.J2L.Layers[CurrentLayer];
        }

    }