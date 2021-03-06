﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace programowanie_SSprint
{
    public partial class PictureEditor : Form, IErrorable, ICommunicative
    {
        #region EVENTS

        public event Action<IErrorable, ICommunicative, picture> insertPicture; //jesli color.id==null, to dodaje nowy obraz, jeśli !=null to aktualizuje istniejący. Zwraca bool czy się udało
        public event Action<IErrorable, ICommunicative> getAllPictures; //zwraca listę wszystkich obrazow
        public event Action<IErrorable, ICommunicative, picture> removePicture; //usuwa obraz. Istotne jest tylko picture.id. Zwraca bool czy się udało


        #endregion
        public PictureEditor()
        {
            InitializeComponent();
            currentlySelectedPicture = null;
            currentlyEditedPicture = new picture();
            btnChooseSelected.Visible = false;
        }
        public picture SelectedPicture { get { return CurrentlySelectedPicture; } }

        public void ShowError(string message, string longMessage = null, string title = null)
        {
            var ErrorWindow = new Views.HelperViews.Error(message, longMessage, title);
            ErrorWindow.ShowDialog();
        }
        public void PushNotification(string text, int type = 0)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                notificationPanel1.PushNotification(text, type);
            }));
          
        }
        public void ReturnListOfObjects(List<object> obj)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                _ReturnListOfObjects(obj);
            }));
        }

        #region PRIVATE
        private picture currentlyEditedPicture;
        private picture currentlySelectedPicture;

        private picture CurrentlySelectedPicture
        {
            get { return currentlySelectedPicture; }
            set
            {
                currentlySelectedPicture = value;
                DisplaySinglePicture(currentlySelectedPicture);
            }
        }
        private void _ReturnListOfObjects(List<object> obj)
        {
            List<picture> recievedPictures = obj.OfType<picture>().ToList();
            if (recievedPictures != null)
            {
                DisplayPictureList(recievedPictures);
                return;
            }
        }


        private void RefreshPictureList()
        {
            getAllPictures(this, this);
        }

        private void DisplayPictureList(List<picture> theList)
        {
            lvPictures.Items.Clear();

            ListViewItem item;
            foreach (var c in theList)
            {
                item = new ListViewItem(c.id.ToString());
                item.Tag = c;
                item.SubItems.Add(c.name);
                item.SubItems.Add(""); //TODO do poprawki
                lvPictures.Items.Add(item);
            }

        }

        private void DisplaySinglePicture(picture p)
        {
            currentlyEditedPicture = p;
            if (p != null)
            {
                tbName.Text = p.name;
                tbID.Text = p.id.ToString();
                //TODO: dodać wyświetlanie obrazka
            }
            else
            {
                //wyswietlanie pustego obrazka
                tbName.Text = "";
                tbID.Text = "";
            }
            return;
        }
        #endregion

        #region GENERATED_EVENTS
        private void PictureEditor_Load(object sender, EventArgs e)
        {
            if (this.Modal)
            {
                //opened as dialog
                btnChooseSelected.Visible = true;
            }
            else
                btnChooseSelected.Visible = false;

            RefreshPictureList();

        }
        private void btnChooseSelected_Click(object sender, EventArgs e)
        {
            if (CurrentlySelectedPicture == null)
            {
                ShowError("Wygląda na to, że żaden obrazek nie jest zaznaczony.");
                return;
            }

            this.Close();
        }
        private void lvPictures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPictures.SelectedItems.Count <= 0 || lvPictures.SelectedItems[0].Tag == null) return; //nic nie jest zaznaczone

            CurrentlySelectedPicture = lvPictures.SelectedItems[0].Tag as picture;
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (tbName.Text.Length <= 0)
            {
             errorProvider1.SetError(tbName, "Wartośc nie może byc pusta");
             return;
            }//error

            currentlyEditedPicture.name = tbName.Text;
        }

        private void btnApplyChanges_Click(object sender, EventArgs e)
        {
            foreach (Control c in groupBoxEditArea.Controls)
            {
                if (errorProvider1.GetError(c).Length > 0) return;
            }
            if (currentlyEditedPicture == null) return;
            insertPicture(this, this, currentlyEditedPicture);
            groupBoxGraphicList.Visible = true;
            RefreshPictureList();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //wypada dać potwierdzenie
            DialogResult dialogResult = MessageBox.Show("Czy na pewno chcesz odrzucić wprowadzone zmiany?", "Potwierdzenie odrzucenia zmian", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) return;

            groupBoxGraphicList.Visible = true;
            currentlyEditedPicture = null;
            CurrentlySelectedPicture = CurrentlySelectedPicture; //odswiezenie
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            CurrentlySelectedPicture = null;
            currentlyEditedPicture = new picture();
            groupBoxGraphicList.Visible = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if (CurrentlySelectedPicture == null)
            {
                ShowError("Wygląda na to, że żaden obrazek nie jest zaznaczony");
                return;
            }

            removePicture(this, this, CurrentlySelectedPicture);
        }
        #endregion

       
    }
}
