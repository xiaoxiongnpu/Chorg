﻿using Caliburn.Micro;
using Chorg.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Chorg.ViewModels
{
    public class EditChartViewModel : Screen
    {
        private Chart chartModel;

        private ContentType Content
        {
            get => chartModel.Content;
            set => chartModel.Content = value; 
        }

        #region ContentBools
        public bool IsGeneral
        {
            get => Content == ContentType.GENERAL;
            set { if (value) Content = ContentType.GENERAL; }
        }

        public bool IsTaxi
        {
            get => Content == ContentType.TAXI;
            set { if (value) Content = ContentType.TAXI; }
        }

        public bool IsSid
        {
            get => Content == ContentType.SID;
            set { if (value) Content = ContentType.SID; }
        }

        public bool IsStar
        {
            get => Content == ContentType.STAR;
            set { if (value) Content = ContentType.STAR; }
        }

        public bool IsApp
        {
            get => Content == ContentType.APP;
            set { if (value) Content = ContentType.APP; }
        }
        #endregion

        public string Description
        {
            get => chartModel.Description;
            set => chartModel.Description = value;
        }

        public string Identifier
        {
            get => chartModel.Identifier;
            set => chartModel.Identifier = value; 
        }

        public ObservableCollection<string> Keywords
        {
            get => chartModel.Keywords == null ? new ObservableCollection<string>() : new ObservableCollection<string>(chartModel.Keywords);
        }

        public bool HasKeywords
        {
            get => Keywords.Count > 0;
        }

        private string _NewKeyword;
        public string NewKeyword
        {
            get { return _NewKeyword; }
            set { _NewKeyword = value; NotifyOfPropertyChange(() => NewKeyword); }
        }

        public EditChartViewModel(Chart chart)
        {
            // Model and ViewModel
            chartModel = chart;

            // Properties of Model
            Content = chart.Content;
            Description = chart.Description;
            Identifier = chart.Identifier;
            Keywords.Replace(chart.Keywords);

            // Update Property "HasKeywords"
            Keywords.CollectionChanged += (o, e) => { NotifyOfPropertyChange(() => HasKeywords);  };
        }

        public void DeleteKeyword(string keyword)
        {
            chartModel.Keywords.Remove(keyword);
            NotifyOfPropertyChange(() => Keywords);
        }
        
        public void AddKeyword()
        {
            if (!string.IsNullOrWhiteSpace(NewKeyword))
            {
                if (!Keywords.Contains(NewKeyword))
                {
                    chartModel.Keywords.Add(NewKeyword);
                    NotifyOfPropertyChange(() => Keywords);
                }
            }
            NewKeyword = null;
        }

        public void EnterOnNewKeyword(KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
                AddKeyword();
        }

        private bool _ConfirmPending;
        public bool ConfirmPending
        {
            get { return _ConfirmPending; }
            set { _ConfirmPending = value; NotifyOfPropertyChange(() => ConfirmPending); }
        }

        /// <summary>
        /// Deletes the chart from the DB
        /// </summary>
        public async void Delete()
        {
            if (!ConfirmPending)
                ConfirmPending = true;

            else
            {
                try
                {
                    // Remove from Database
                    await Gateway.GetInstance().DeleteChartAsync(chartModel);

                    // Remove from Chart Editor
                    (Parent as EditChartsViewModel).ChartThumbs.Remove(chartModel);
                }
                catch (Exception e)
                {
                    MainViewModel.GetInstance().TriggerSnackbar(e);
                }
            }
        }
    }
}
