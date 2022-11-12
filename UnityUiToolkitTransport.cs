using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AnchorLinkSharp;
using Assets.Packages.AnchorLinkTransportSharp.UI.Example;
using Assets.Packages.AnchorLinkTransportSharp.UI.ScriptsAndUxml;
using EosioSigningRequest;
using EosSharp.Core.Api.v1;
using HyperionApiClient.Responses;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using Debug = UnityEngine.Debug;

namespace Assets.Packages.AnchorLinkTransportSharp
{
    public class UnityUiToolkitTransport : UnityTransport
    {
        [SerializeField] internal SuccessOverlayView SuccessOverlayView;
        [SerializeField] internal FailureOverlayView FailureOverlayView;
        [SerializeField] internal QrCodeOverlayView QrCodeOverlayView;
        [SerializeField] internal LoadingOverlayView LoadingOverlayView;
        [SerializeField] internal SigningTimerOverlayView SigningTimerOverlayView;
        [SerializeField] internal TimeoutOverlayView TimeoutOverlayView;
        [SerializeField] internal LoginView LoginView;

        // BASE-CLASS HAS FOLLOWING FIELDS
        //private readonly bool _requestStatus;
        //private readonly bool _fuelEnabled;
        //private SigningRequest _activeRequest;
        //private object _activeCancel; //?: (reason: string | Error) => void
        //private Timer _countdownTimer;
        //private Timer _closeTimer;

     
        private AnchorLink _anchorLink;
        private Coroutine _countdownTimerRoutine;

        public LinkSession LinkSession;

        private const string Identifier = "example";

        public string ESRLink = ""; // Link that will be converted to a QR code and can be copy from
        public UnityUiToolkitTransport(TransportOptions options) : base(options)
        {
        }


        public async Task StartSession()
        {
            _anchorLink = new AnchorLink(new LinkOptions()
            {
                Transport = this,
                ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
                Rpc = "https://eos.greymass.com",
                ZlibProvider = new NetZlibProvider(),
                Storage = new JsonLocalStorage()
            });

            try
            {
                var loginResult = await _anchorLink.Login(Identifier);

                LinkSession = loginResult.Session;
                Debug.Log($"{LinkSession.Auth.actor} logged-in");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        // tries to restore session, called when document is loaded
        public async Task RestoreSession()
        {
            var restoreSessionResult = await _anchorLink.RestoreSession(Identifier);
            LinkSession = restoreSessionResult;

            if (LinkSession != null)
                Debug.Log($"{LinkSession.Auth.actor} logged-in");
        }


        // transfer tokens using a session  
        public async Task Transfer(EosSharp.Core.Api.v1.Action action)
        {
            var transactResult = await LinkSession.Transact(new TransactArgs() { Action = action });

            print($"Transaction broadcast! {transactResult.Processed}");
        }


        // see https://github.com/greymass/anchor-link-browser-transport/blob/master/src/index.ts#L361
        // and https://github.com/greymass/anchor-link-console-transport/blob/master/src/index.ts#L10
        public override void ShowLoading()
        {
            LoadingOverlayView.Show();
        }

        // see https://github.com/greymass/anchor-link-browser-transport/blob/master/src/index.ts#L680
        public override void OnSuccess(SigningRequest request, TransactResult result)
        {
            QrCodeOverlayView.Hide();

            if (request == _activeRequest)
            {
                SuccessOverlayView.Show();
                SuccessOverlayView.CloseTimer();
            }

        }

        // see https://github.com/greymass/anchor-link-browser-transport/blob/master/src/index.ts#L698
        public override void OnFailure(SigningRequest request, Exception exception)
        {
            if (request == _activeRequest)
            {
                FailureOverlayView.Show();
                FailureOverlayView.ExceptionHandler(exception);
            }
            else
            {
                FailureOverlayView.Hide();
            }
        }

        // see https://github.com/greymass/anchor-link-browser-transport/blob/master/src/index.ts#L264
        public override void DisplayRequest(SigningRequest request)
        {
            //This method is called when the anchor launch is ready to be clicked?
            ESRLink = request.Encode(false, false);  // This returns ESR link to be converted
            var qrCodeTexture = StringToQRCodeTexture2D(ESRLink);

            QrCodeOverlayView.Show();
            QrCodeOverlayView.Rebind(qrCodeTexture, true, false);
        }

        // see https://github.com/greymass/anchor-link-browser-transport/blob/master/src/index.ts#L226
        public override void ShowDialog(string title = null, string subtitle = null, string type = null, Action action = null,
            object content = null)
        {
            print("#########################################");
        }

    }
}
