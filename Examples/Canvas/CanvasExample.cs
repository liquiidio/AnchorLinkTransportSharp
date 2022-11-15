using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnchorLinkSharp;
using Assets.Packages.AnchorLinkTransportSharp.Src;
using Assets.Packages.AnchorLinkTransportSharp.Src.StorageProviders;
using Assets.Packages.AnchorLinkTransportSharp.Src.Transports.Canvas;
using EosSharp.Core.Api.v1;
using TMPro;
using UnityEngine;

namespace Assets.Packages.AnchorLinkTransportSharp.Examples.Canvas
{
    public class CanvasExample : MonoBehaviour
    {
        // Assign UnityTransport through the Editor
        [SerializeField] internal UnityCanvasTransport Transport;

        public GameObject CustomLoginPanel;
        public GameObject CustomTransferPanel;

        // app identifier, should be set to the eosio contract account if applicable
        private const string Identifier = "example";

        // initialize the link
        private AnchorLink _link;

        // the session instance, either restored using link.restoreSession() or created with link.login()
        private LinkSession _session;

        private void Start()
        {
            CustomLoginPanel.SetActive(true);
            CustomTransferPanel.SetActive(false);
        }

        public async void StartSession()
        {
            _link = new AnchorLink(new LinkOptions()
            {
                Transport = this.Transport,
                //ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
                //Rpc = "https://eos.greymass.com",
                ChainId = "1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4",
                Rpc = "https://wax.greymass.com",
                ZlibProvider = new NetZlibProvider(),
                Storage = new JsonLocalStorage()
                //chains: [{
                //    chainId: 'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906',
                //    nodeUrl: 'https://eos.greymass.com',
                //}]
            });

            await Login();
        }

        // login and store session if sucessful
        public async Task Login()
        {
            var loginResult = await _link.Login(Identifier);
            _session = loginResult.Session;
            DidLogin();
        }

        // tries to restore session, called when document is loaded
        public async Task RestoreSession()
        {
            var restoreSessionResult = await _link.RestoreSession(Identifier);
            _session = restoreSessionResult;
            if (_session != null)
                DidLogin();
        }

        // logout and remove session from storage
        public async Task Logout()
        {
            await _session.Remove();
        }

        // called when session was restored or created
        public void DidLogin()
        {
            Debug.Log($"{_session.Auth.actor} logged-in");

            Invoke(nameof(ShowCustomTransferPanel), 1.5f);
        }

        private void ShowCustomTransferPanel ()
        {
            Transport.DisableCurrentPanel(CustomTransferPanel);
        }

        // transfer tokens using a session
        public async Task Transfer(string frmAcc, string toAcc, string qnty, string memo)
        {
            var action = new EosSharp.Core.Api.v1.Action()
            {
                account = "eosio.token",
                name = "transfer",
                authorization = new List<PermissionLevel>() { _session.Auth },
                data = new Dictionary<string, object>()
                {
                    {"from", frmAcc},
                    {"to", toAcc},
                    {"quantity", qnty},
                    {"memo", memo}
                }
            };

            var transactResult = await _session.Transact(new TransactArgs() { Action = action });
           Debug.Log($"Transaction broadcast! {transactResult.Processed}");
        }


        //private AnchorLink _anchorLink;
        //private LinkSession _linkSession;

        //private const string Identifier = "example";

        //public async void StartSession()
        //{
        //    _anchorLink = new AnchorLink(new LinkOptions()
        //    {
        //        Transport = this,
        //        ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
        //        Rpc = "https://eos.greymass.com",
        //        ZlibProvider = new NetZlibProvider(),
        //        Storage = new JsonLocalStorage()
        //    });

        //    try
        //    {
        //        await CreateSession();
        //    }

        //    catch (Exception ex)
        //    {
        //        Debug.LogError(ex);
        //    }
        //}

        //public async Task CreateSession()
        //{
        //    try
        //    {
        //        var loginResult = await _anchorLink.Login(Identifier);

        //        _linkSession = loginResult.Session;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError(ex);
        //    }
        //}

        // tries to restore session, called when document is loaded
        //public async Task RestoreSession()
        //{
        //    var restoreSessionResult = await _anchorLink.RestoreSession(Identifier);
        //    _linkSession = restoreSessionResult;

        //    if (_linkSession != null)
        //        Debug.Log($"{_linkSession.Auth.actor} logged-in");
        //}


        public async void TryTransferTokens(GameObject TransferDetailsPanel)
        {
            string _frmAcc = "";
            string _toAcc = "";
            string _qnty = "";
            string _memo = "";

            foreach (var _inputField in TransferDetailsPanel.GetComponentsInChildren<TMP_InputField>())
            {
                if (_inputField.name == "FromAccountInputField(TMP)")
                    _frmAcc = _inputField.text;

                else if (_inputField.name == "ToAccountInputField(TMP)")
                    _toAcc = _inputField.text;

                else if (_inputField.name == "QuantityAccountInputField(TMP)")
                    _qnty = $"{_inputField.text} WAX";

                else if (_inputField.name == "MemoAccountInputField(TMP)")
                    _memo = _inputField.text;
            }

            await Transfer
            (
                _frmAcc,
                _toAcc,
                _qnty,
                _memo
            );

            //CustomTransferPanel.SetActive( false );
            //await Transfer();
        }

        //// transfer tokens using a session  // For testing
        //public async Task Transfer2(string fromAccount = "", string toAccount = "teamgreymass", string quanitiy = "0.0001 EOS", string memo = "Anchor is the best! Thank you <3")
        //{
        //    var action = new EosSharp.Core.Api.v1.Action()
        //    {
        //        account = "eosio.token",
        //        name = "transfer",
        //        authorization = new List<PermissionLevel>() { _session.Auth },
        //        data = new Dictionary<string, object>()
        //            {
        //                { "from", fromAccount == "" ?  _session.Auth.actor: fromAccount},
        //                { "to", toAccount },
        //                { "quantity", quanitiy },
        //                { "memo", memo }
        //            }
        //    };

        //    var transactResult = await _session.Transact(new TransactArgs() { Action = action });
        //   Debug.Log($"Transaction broadcast! {transactResult.Processed}");
        //}

        //// transfer tokens using a session
        //public async Task Transfer()
        //{
        //    var action = new EosSharp.Core.Api.v1.Action()
        //    {
        //        account = "eosio.token",
        //        name = "transfer",
        //        authorization = new List<PermissionLevel>() { _session.Auth },
        //        data = new Dictionary<string, object>()
        //            {
        //                { "from", _session.Auth.actor },
        //                { "to", "teamgreymass" },
        //                { "quantity", "0.0001 EOS" },
        //                { "memo", "Anchor is the best! Thank you <3" }
        //            }
        //    };

        //    var transactResult = await _session.Transact(new TransactArgs() { Action = action });
        //   Debug.Log($"Transaction broadcast! {transactResult.Processed}");
        //}
    }
}

