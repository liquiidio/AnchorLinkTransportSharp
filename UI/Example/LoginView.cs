using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnchorLinkSharp;
using Assets.Packages.AnchorLinkTransportSharp;
using EosSharp.Core.Api.v1;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Packages.AnchorLinkTransportSharp.UI.Example
{
    public class LoginView : ScreenBase
    {

        /*
         * Child-Controls
         */

        private Button _loginButton;

        private Label _versionLabel;


        /*
         * Fields, Properties
         */
        [SerializeField]public UnityUiToolkitTransport UnityUiToolkitTransport;
        [SerializeField]private LoggedView LoggedView;

        void Start()
        {
            _loginButton = Root.Q<Button>("login-button");
            _versionLabel = Root.Q<Label>("version-label");

            _versionLabel.text = Version;

            BindButtons();
            Show();
        }


        #region Button Binding
        private void BindButtons()
        {
            _loginButton.clickable.clicked +=  async () =>
            {
                try
                {
                   await UnityUiToolkitTransport.StartSession();
                   LoggedView.Show();
                   LoggedView.Rebind();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                Hide();
            };

            _versionLabel.RegisterCallback<ClickEvent>(evt =>
            {
                Application.OpenURL(VersionUrl);
            });
        }
        #endregion
    }
}
