/*
 * Copyright (c) 2009, Peter Nelson (charn.opcode@gmail.com)
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
*/

// Basic implementation for handling events when resources are downloaded.  More info at
// http://developer.apple.com/documentation/Cocoa/Reference/WebKit/Protocols/WebResourceLoadDelegate_Protocol

using System;
using System.Collections.Generic;
using System.Text;
using WebKit;
using WebKit.Interop;

namespace WebKit
{
    // Delegate definitions WebFrameLoadDelegate events
    internal delegate void DidResourceCancelAuthenticationChallengeEvent(WebView WebView, uint identifier, IWebURLAuthenticationChallenge challenge, IWebDataSource dataSource);
    internal delegate void DidResourceFailLoadWithErrorEvent(WebView WebView, uint identifier, WebError error, IWebDataSource dataSource);
    internal delegate void DidResourceFinishLoadFromDataSourceEvent(WebView WebView, uint identifier, IWebDataSource dataSource);
    internal delegate void DidResourceReceiveAuthenticationChallengeEvent(WebView WebView, uint identifier, IWebURLAuthenticationChallenge challenge, IWebDataSource dataSource);
    internal delegate void DidResourceReceiveDataOfLengthEvent(WebView WebView, uint identifier, uint length, IWebDataSource dataSource);
    internal delegate void DidResourceReceiveResponseEvent(WebView WebView, uint identifier, WebURLResponse response, IWebDataSource dataSource);
    internal delegate void ResourceIdentifierForInitialRequestEvent(WebView WebView, IWebURLRequest request, IWebDataSource dataSource, uint identifier);
    internal delegate void ResourcePlugInFailedWithErrorEvent(WebView WebView, WebError error, IWebDataSource dataSource);
    internal delegate void WillResourceSendRequestEvent(WebView WebView, uint identifier, IWebURLRequest request, WebURLResponse redirectResponse, IWebDataSource dataSource, out IWebURLRequest output);

    internal class WebResourceLoadDelegate : IWebResourceLoadDelegate
    {
        public event DidResourceCancelAuthenticationChallengeEvent DidCancelAuthenticationChallenge = delegate { };
        public event DidResourceFailLoadWithErrorEvent DidFailLoadingWithError = delegate { };
        public event DidResourceFinishLoadFromDataSourceEvent DidFinishLoadFromDataSource = delegate { };
        public event DidResourceReceiveAuthenticationChallengeEvent DidReceiveAuthenticationChallenge = delegate { };
        public event DidResourceReceiveDataOfLengthEvent DidReceiveContentLength = delegate { };
        public event DidResourceReceiveResponseEvent DidReceiveResponse = delegate { };
        public event ResourceIdentifierForInitialRequestEvent IdentifierForInitialRequest = delegate { };
        public event ResourcePlugInFailedWithErrorEvent PlugInFailedWithError = delegate { };
        public event WillResourceSendRequestEvent WillSendRequest = delegate(WebView WebView, uint identifier, IWebURLRequest request, WebURLResponse redirectResponse, IWebDataSource dataSource, out IWebURLRequest output) { output = request; };

        #region IWebResourceLoadDelegate Members

        public void didCancelAuthenticationChallenge(WebView WebView, uint identifier, IWebURLAuthenticationChallenge challenge, IWebDataSource dataSource)
        {
            DidCancelAuthenticationChallenge(WebView, identifier, challenge, dataSource);
        }

        public void didFailLoadingWithError(WebView WebView, uint identifier, WebError error, IWebDataSource dataSource)
        {
            DidFailLoadingWithError(WebView, identifier, error, dataSource);
        }

        public void didFinishLoadingFromDataSource(WebView WebView, uint identifier, IWebDataSource dataSource)
        {
            DidFinishLoadFromDataSource(WebView, identifier, dataSource);
        }

        public void didReceiveAuthenticationChallenge(WebView WebView, uint identifier, IWebURLAuthenticationChallenge challenge, IWebDataSource dataSource)
        {
            DidReceiveAuthenticationChallenge(WebView, identifier, challenge, dataSource);
        }

        public void didReceiveContentLength(WebView WebView, uint identifier, uint length, IWebDataSource dataSource)
        {
            DidReceiveContentLength(WebView, identifier, length, dataSource);
        }

        public void didReceiveResponse(WebView WebView, uint identifier, WebURLResponse response, IWebDataSource dataSource)
        {
            DidReceiveResponse(WebView, identifier, response, dataSource);
        }

        public void identifierForInitialRequest(WebView WebView, IWebURLRequest request, IWebDataSource dataSource, uint identifier)
        {
            IdentifierForInitialRequest(WebView, request, dataSource, identifier);
        }

        public void plugInFailedWithError(WebView WebView, WebError error, IWebDataSource dataSource)
        {
            PlugInFailedWithError(WebView, error, dataSource);
        }

        public IWebURLRequest willSendRequest(WebView WebView, uint identifier, IWebURLRequest request, WebURLResponse redirectResponse, IWebDataSource dataSource)
        {
            IWebURLRequest o = null;
            WillSendRequest(WebView, identifier, request, redirectResponse, dataSource, out o);
            return o;
        }

        #endregion

    }
}
