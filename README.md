# Family Board

Digital display software that runs on a Raspberry Pi and displays calendars, todo lists via other Microsoft Graph.


## Secrets

Create an `appsettings.secrets.json` file in the root of the `FamilyBoard` project. This file is in `.gitignore` so it won't be picked up by git.

Add the following contents to connect to Azure AD.

```json
{
  "AzureAd": {
    "Instance": "",
    "Domain": "",
    "TenantId": "",
    "ClientId": "",
    "CallbackPath": "",
    "ClientSecret": ""
  }
}
```


## Display

Disable screen time out on Raspbian Desktop

https://stackoverflow.com/questions/30985964/how-to-disable-sleeping-on-raspberry-pi


## Development

Create a dev certificate to trust on your dev environment and Raspberry Pi. You need a `localhost.pfx` file in the project root from a trusted certificate.

https://stackoverflow.com/questions/55485511/how-to-run-dotnet-dev-certs-https-trust

