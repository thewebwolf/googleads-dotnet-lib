// Copyright 2011, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Ads.Common.Config;
using Google.Api.Ads.Common.Lib;
using System;
using System.Collections.Generic;

namespace Google.Api.Ads.Dfp.Lib {
  /// <summary>
  /// This class reads the configuration keys from App.config.
  /// </summary>
  public class DfpAppConfig : AppConfigBase {
    /// <summary>
    /// The short name to identify this assembly.
    /// </summary>
    private const string SHORT_NAME = "DfpApi-DotNet";

    /// <summary>
    /// Default value for DFPAPI_SERVER.
    /// </summary>
    private const string DEFAULT_DFPAPI_SERVER = "https://ads.google.com";

    /// <summary>
    /// The OAuth2 scope for DFP API.
    /// </summary>
    private const string DFP_OAUTH2_SCOPE = "https://www.googleapis.com/auth/dfp";

    /// <summary>
    /// The default value for application name.
    /// </summary>
    public const string DEFAULT_APPLICATION_NAME = "INSERT_YOUR_APPLICATION_NAME_HERE";

    /// <summary>
    /// NetworkCode to be used in SOAP headers.
    /// </summary>
    private ConfigSetting<string> networkCode = new ConfigSetting<string>("NetworkCode", "");

    /// <summary>
    /// Application name to be used in SOAP headers.
    /// </summary>
    private ConfigSetting<string> applicationName = new ConfigSetting<string>(
        "ApplicationName", DEFAULT_APPLICATION_NAME);

    /// <summary>
    /// URL for DFP API.
    /// </summary>
    private ConfigSetting<string> dfpApiServer = new ConfigSetting<string>("DfpApi.Server",
        DEFAULT_DFPAPI_SERVER);

    /// <summary>
    /// Authorization method to be used when making API calls.
    /// </summary>
    private ConfigSetting<DfpAuthorizationMethod> authorizationMethod =
        new ConfigSetting<DfpAuthorizationMethod>(
            "AuthorizationMethod", DfpAuthorizationMethod.OAuth2);

    /// <summary>
    /// Gets or sets networkCode to be used in SOAP headers.
    /// </summary>
    public string NetworkCode {
      get => networkCode.Value;
      set => SetPropertyAndNotify(networkCode, value);
    }

    /// <summary>
    /// Gets or sets application name to be used in SOAP headers.
    /// </summary>
    public string ApplicationName {
      get => applicationName.Value;
      set => SetPropertyAndNotify(applicationName, value);
    }

    /// <summary>
    /// Gets or sets URL for DFP API.
    /// </summary>
    public string DfpApiServer {
      get => dfpApiServer.Value;
      set => SetPropertyAndNotify(dfpApiServer, value);
    }

    /// <summary>
    /// Gets or sets the authorization method to be used when making API calls.
    /// </summary>
    public DfpAuthorizationMethod AuthorizationMethod {
      get => authorizationMethod.Value;
      set => SetPropertyAndNotify(authorizationMethod, value);
    }

    /// <summary>
    /// Gets a useragent string that can be used with the library.
    /// </summary>
    public override string GetUserAgent() {
      return String.Format("{0} ({1}{2})", ApplicationName, Signature,
          EnableGzipCompression ? ", gzip" : "");
    }

    /// <summary>
    /// Public constructor.
    /// </summary>
    public DfpAppConfig() : base() {
      ReadSettings(LoadConfigSection("DfpApi"));
    }

    /// <summary>
    /// Read all settings from App.config.
    /// </summary>
    /// <param name="settings">The parsed App.config settings.</param>
    protected override void ReadSettings(Dictionary<string, string> settings) {
      base.ReadSettings(settings);

      ReadSetting(settings, networkCode);
      ReadSetting(settings, applicationName);
      ReadSetting(settings, dfpApiServer);
      ReadSetting(settings, authorizationMethod);

      // If there is an OAuth2 scope mentioned in App.config, this will be
      // loaded by the above call. If there isn't one, we will initialize it
      // with a library-specific default value.
      if (string.IsNullOrEmpty(OAuth2Scope)) {
        OAuth2Scope = GetDefaultOAuth2Scope();
      }
    }

    /// <summary>
    /// Gets the default OAuth2 scope.
    /// </summary>
    public override string GetDefaultOAuth2Scope() {
      return DFP_OAUTH2_SCOPE;
    }
  }
}
