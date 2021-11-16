# Calendar Display

Blazor Server app to display calendar and todo lists from the Microsoft Graph


## Run via Docker Compose


#### `docker-compose.yml`

```yml
version: '3.9'

services:
  calendardisplay:
    image: 'ghcr.io/alex-oswald/homeinfra-calendardisplay:main'
    container_name: 'calendar_display'
    restart: 'unless-stopped'
    volumes:
      - './configuration/:/configuration/' # cert location
    environment:
      - 'AzureAd__Instance=https://login.microsoftonline.com/'
      - 'AzureAd__TenantId=common'
      - 'AzureAd__ClientId=${AZUREAD_CLIENTID}'
      - 'AzureAd__ClientSecret=${AZUREAD_CLIENTSECRET}'
      - 'ASPNETCORE_ENVIRONMENT=Production'
      - 'ASPNETCORE_URLS=https://+:443'
      - 'ASPNETCORE_Kestrel__Certificates__Default__Password=${CERT_PASSWORD}'
      - 'ASPNETCORE_Kestrel__Certificates__Default__Path=${CERT_PATH}'
      - 'TodoListOptions__UpdateFrequency=3600'
      - 'TodoListOptions__TodoLists__2__Name=${TODOLISTS_2_NAME}'
      - 'TodoListOptions__TodoLists__1__Name=${TODOLISTS_1_NAME}'
      - 'TodoListOptions__TodoLists__0__Name=${TODOLISTS_0_NAME}'
      - 'CountdownOptions__UpdateFrequency=86400'
      - 'CountdownOptions__LookupMonths=24'
      - 'CountdownOptions__CountdownCount=3'
      - 'CountdownOptions__CalendarName=Vacations'
      - 'CalendarOptions__UpdateFrequency=3600'
      - 'CalendarOptions__Calendars__7__TextColor=black'
      - 'CalendarOptions__Calendars__7__Name=${CALENDARS_7_NAME}'
      - 'CalendarOptions__Calendars__7__BackgroundColor=limegreen'
      - 'CalendarOptions__Calendars__6__TextColor=white'
      - 'CalendarOptions__Calendars__6__Name=${CALENDARS_6_NAME}'
      - 'CalendarOptions__Calendars__6__BackgroundColor=deeppink'
      - 'CalendarOptions__Calendars__5__TextColor=black'
      - 'CalendarOptions__Calendars__5__Name=${CALENDARS_5_NAME}'
      - 'CalendarOptions__Calendars__5__BackgroundColor=turquoise'
      - 'CalendarOptions__Calendars__4__TextColor=white'
      - 'CalendarOptions__Calendars__4__Name=${CALENDARS_4_NAME}'
      - 'CalendarOptions__Calendars__4__BackgroundColor=orangered'
      - 'CalendarOptions__Calendars__3__TextColor=black'
      - 'CalendarOptions__Calendars__3__Name=Vacations'
      - 'CalendarOptions__Calendars__3__BackgroundColor=yellow'
      - 'CalendarOptions__Calendars__2__TextColor=white'
      - 'CalendarOptions__Calendars__2__Name=Seattle Seahawks'
      - 'CalendarOptions__Calendars__2__BackgroundColor=navy'
      - 'CalendarOptions__Calendars__1__TextColor=white'
      - 'CalendarOptions__Calendars__1__Name=United States holidays'
      - 'CalendarOptions__Calendars__1__BackgroundColor=maroon'
      - 'CalendarOptions__Calendars__0__TextColor=white'
      - 'CalendarOptions__Calendars__0__Name=Calendar'
      - 'CalendarOptions__Calendars__0__BackgroundColor=indigo'
```


#### `.env`

Fill out the values.

```
AZUREAD_CLIENTID=
AZUREAD_CLIENTSECRET=
CERT_PASSWORD=
CERT_PATH=/configuration/server.pfx
TODOLISTS_2_NAME=
TODOLISTS_1_NAME=
TODOLISTS_0_NAME=
CALENDARS_7_NAME=
CALENDARS_6_NAME=
CALENDARS_5_NAME=
CALENDARS_4_NAME=
```


## Disable screen timeout

Disable screen time out on Raspbian Desktop

https://stackoverflow.com/questions/30985964/how-to-disable-sleeping-on-raspberry-pi
