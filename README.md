# Calendar Display

Digital display software that runs on a Raspberry Pi and displays Microsoft Graph calendar and todo list data.


## Setup

1. Install Raspian on an SD card. Add WiFi info.
2. Update & upgrade.
3. [Install dotnet](https://docs.microsoft.com/en-us/dotnet/iot/deployment).
4. Add dotnet to sudo (may not be required?).
7. [Disable screen timeout](#Disable-screen-timeout)
5. [Add secrets file with your values](#Secrets).
6. [Generate dev certificate](#Create-development-certificate).
8. Publish app
9. [Copy files to Raspberry Pi](#Copy-files-to-Raspberry-Pi)
10. Run App
11. Navigate in browser & login


## Disable screen timeout

Disable screen time out on Raspbian Desktop

https://stackoverflow.com/questions/30985964/how-to-disable-sleeping-on-raspberry-pi


## Secrets

Create an `appsettings.secrets.json` file in the root of the `CalendarDisplay` project. This file is in `.gitignore` so it won't be picked up by git.

Add the following contents to connect to Azure AD.

```json
{
  "AzureAd": {
    "Instance": "",
    "TenantId": "",
    "ClientId": "",
    "CallbackPath": "",
    "ClientSecret": ""
  }
}
```


## Create development certificate

Create a dev certificate to trust on your dev environment and Raspberry Pi. You need a `localhost.pfx` file in the project root from a trusted certificate.

https://stackoverflow.com/questions/55485511/how-to-run-dotnet-dev-certs-https-trust


## Copy files to Raspberry Pi

Create a `CalendarDisplay` folder and secure copy to the pi.

```bash
scp -r C:\{project-root}\bin\Release\net5.0\publish\* pi@rasbpian:/home/pi/CalendarDisplay
```