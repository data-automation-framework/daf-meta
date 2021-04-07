# Data Automation Framework - Meta (Daf Meta)
**Note: This project is currently in an alpha state and should be considered unstable. Breaking changes to the public API will occur.**

Daf is a plugin based data and integration automation framework primarily designed to facilitate data warehouse and ETL processes. Developers use this framework to programatically generate data integration objects using the Daf templating language.

Daf Meta is a Data Vault 2.0 metadata library & editor, designed to be used with Daf Core. The editor is a Windows application in which users can set up data sources, data vault structures and mappings from source tables or files to data vault tables. 

## Installation
The editor application requires the following:
Windows 10
.NET 5.0 runtime

Download the zip archive from the release page of this repository and extract the contents.

The core library is built in .NET 5.0 and is cross platform.

## Usage
Run the executable file extracted from the zip archive. This opens a new metadata editor:

![dafmeta](https://user-images.githubusercontent.com/1073539/112892917-3f0cb600-90da-11eb-9fd5-f2237d052683.png)

Setup connections, data source names, tenants and the target data vault in the options in the Configuration menu. 

Add new sources in the main window and add column mappings to the staging tables and raw data vault.

## Links
[Daf organization](https://github.com/data-automation-framework)

[Documentation](https://data-automation-framework.com)

## Collaborators

<table>
    <tr>
        <td align="center">
            <a href="https://github.com/bjorhn">
                <img src="https://avatars.githubusercontent.com/u/421316?v=4" width="100;" alt="bjorhn"/>
                <br />
                <sub><b>Oscar Björhn</b></sub>
            </a>
        </td>
        <td align="center">
            <a href="https://github.com/petlofg">
                <img src="https://avatars.githubusercontent.com/u/1073539?v=4" width="100;" alt="petlofg"/>
                <br />
                <sub><b>Petter Löfgren</b></sub>
            </a>
        </td>
    </tr>
</table>

## Contributors

<table>
<tr>
    <td align="center">
        <a href="https://github.com/vholmer">
            <img src="https://avatars.githubusercontent.com/u/5623158?v=4" width="100;" alt="vholmer"/>
            <br />
            <sub><b>Viktor Holmér</b></sub>
        </a>
    </td>
    <td align="center">
        <a href="https://github.com/rmodig">
            <img src="https://avatars.githubusercontent.com/u/74718894?v=4" width="100;" alt="rmodig"/>
            <br />
            <sub><b>Rickard Modig</b></sub>
        </a>
    </td>
</table>
