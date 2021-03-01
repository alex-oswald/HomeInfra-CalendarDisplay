# Family Board

Digital display software that runs on a Raspberry Pi and displays calendars, todo lists via other Microsoft Graph.


## Setup

1. Install Raspian on an SD card. Add WiFi info.
2. Update & upgrade.
3. [Install dotnet](https://docs.microsoft.com/en-us/dotnet/iot/deployment).
4. Add dotnet to sudo.
7. [Disable screen timeout](#disable-screen)
5. [Add secrets file with your values](#secrets).
6. [Generate dev certificate](#dev-cert).
8. Publish app
9. [Copy files to Raspberry Pi](#copy-files)
10. Run App
11. Navigate in browser & login


## Disable screen timeout {#disable-screen}

Disable screen time out on Raspbian Desktop

https://stackoverflow.com/questions/30985964/how-to-disable-sleeping-on-raspberry-pi


## Secrets {#secrets}

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


## Create development certificate {#dev-cert}

Create a dev certificate to trust on your dev environment and Raspberry Pi. You need a `localhost.pfx` file in the project root from a trusted certificate.

https://stackoverflow.com/questions/55485511/how-to-run-dotnet-dev-certs-https-trust


## Copy files to Raspberry Pi {#copy-files}

Create a `FamilyBoard` folder and secure copy to the pi.

```bash
scp -r C:\{project-root}\bin\Release\net5.0\publish\* pi@rasbpian:/home/pi/FamilyBoard
```