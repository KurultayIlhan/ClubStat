# ClubStat

Hi, I'm Ilhan Kurultay, and this is my project, ClubStat.

## Overview

ClubStat is an app designed to help players record their location and speed to improve and become better players. The app features two types of profiles: Player and Coach.

ClubStat utilizes an Identity Management System (IMS), meaning only admins can create accounts. This prevents the creation of fake profiles to cheat in the games. Each season, clubs that wish to participate will send an email with the players' details and photos. The app makes it easy to add players, coaches, change passwords, and retrieve their stats via Swagger: [ClubStat Swagger Documentation](https://ilhankurultay-001-site1.btempurl.com/swagger/index.html).

## Getting Started

To start using ClubStat, clone the repository to your computer. The recommended directory is:

This will keep the project organized and easily accessible.

### Prerequisites

- Visual Studio (recommended for opening the solution and managing the project)

### Project Structure

Upon opening the solution in Visual Studio, you will find the following folders:

1. **ClubStat.Infrastructure**
2. **ClubStat.Infrastructure.Tests**
3. **ClubStat.RestServer**
4. **ClubStat.RestServerTests**
5. **ClubStatDataBase**
6. **ClubStatUI**
7. **ClubStatUITests**
8. **ClubStat.IntegrationTest**

### ClubStatUI

The main folder you will need is **ClubStatUI**. This is the front-end part of the project where the ViewModel provides information to the view, and the XAML files display it.

To use the app on your phone, follow these steps:

#### Enabling Developer Options on Android

First, ensure your phone has Developer Options enabled. If not, you can easily enable it by following this guide: [How to Enable Developer Options on Android](https://www.digitaltrends.com/mobile/how-to-get-developer-options-on-android/)

[![Enable Developer Options](https://img.shields.io/badge/Enable_Developer_Options-000?style=for-the-badge&logo=android&logoColor=white)](https://www.digitaltrends.com/mobile/how-to-get-developer-options-on-android/)

#### Connecting Your Phone

1. Connect your phone to your computer.
2. Build the project for your phone in Visual Studio.
3. Once the build is complete, you can use the app on your phone wherever you go.

---

By following these instructions, you should be able to set up and use ClubStat effectively. If you have any questions or need further assistance, feel free to reach out.

Happy training!

Ilhan Kurultay
