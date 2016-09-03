using System;
using System.ComponentModel;
using System.Deployment.Application;

namespace Agent.Model
{
    public class SilentUpdater : INotifyPropertyChanged // скрытое обновление
    {
        private AgentSystem m_agent;
        private readonly ApplicationDeployment applicationDeployment; // Ссылка на приложение (ClickOnce)
        private readonly System.Timers.Timer timer = new System.Timers.Timer(60000); // таймер проверки обновления
        private bool processing;
        public event EventHandler<EventArgs> Completed; // обновление завершено
        public event PropertyChangedEventHandler PropertyChanged;
        private bool updateAvailable;

        public bool UpdateAvailable
        {
            get { return updateAvailable; }
            private set
            {
                updateAvailable = value;
                OnPropertyChanged("UpdateAvailable");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnCompleted()
        {
            var handler = Completed;
            if (handler != null)
            {
                Completed(this, null);
            }
        }

        public SilentUpdater(AgentSystem agent)
        {
            m_agent = agent;
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                return;
            }
            applicationDeployment = ApplicationDeployment.CurrentDeployment;
            applicationDeployment.CheckForUpdateCompleted += CheckForUpdateCompleted;
            applicationDeployment.UpdateCompleted += UpdateCompleted;
            timer.Elapsed += (sender, args) => {
                if (processing)
                {
                    return;
                }
                processing = true;
                applicationDeployment.CheckForUpdateAsync();
            };
            timer.Start();
        }
        
        private void CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled || !e.UpdateAvailable)
            {
                processing = false;
                return;
            }
            applicationDeployment.UpdateAsync();
        }
        
        private void UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            processing = false;
            if (e.Cancelled || e.Error != null)
            {
                Log.Write("Could not install the latest version of the application.");
                return;
            }
            UpdateAvailable = true;
            OnCompleted();
            if(m_agent.Status.Free==true)
                Programm.Reset();
        }

    }
}
