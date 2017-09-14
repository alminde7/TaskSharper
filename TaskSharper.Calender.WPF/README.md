# TaskSharper.Calendar.WPF

This project defines structure and behaviour for **TaskSharper Calendar**

## Project structure
This section describes various quirks to be aware of when working on this project.

### PRISM ViewModelLocator
To enable ViewModelLocator these lines has to be put into a view:

```xaml
xmlns:prism="http://prismlibrary.com/"
prism:ViewModelLocator.AutoWireViewModel="True"
```
The default behaviour for the ViewModelLocator is to look for views in `Views` namespace and viewmodels in `ViewModels` namespace. However, if a different 
folderstructure is preferred, the namespace for both views and viewmodels has to be changed to their corresponding correct namspaces in order for ViewModelLocator 
to resolve views and viewmodels. 

## Setup Google Authentication
In order to connect the application to Google, an authentication file must be provided. This file contains a secret which must not be known to everyone but the developers.
This file is therefor not controlled by Git.

To succesfully startup the application with Google Calendar Authentication, there are certain steps to be taken: 

1. Go to OneNote page _Bachelor -> Documentation -> Google API authentication_
2. Download the file called _Client_secret.json_
3. Copy this file into the root of _TaskSharper.Calendar.WPF_ (in the same directory as the .csproj file)
4. Open Visual Studio and import the file into the project _TaskSharper.Calendar.WPF_
5. Click on the file and open its properties
6. Set `Copy to Output Directory` to `Copy always`

The first time this application is started on a new computer, Google will at startup prompt for a Google Account (username/password). The application will save a file containing 
a _client secret_ and a _refresh token_, eliminating the need to specifiy an account once again in the future.

