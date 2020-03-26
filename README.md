# BeckhoffS7Client

[![Build status](https://ci.appveyor.com/api/projects/status/7qbba540vvgwj09a?svg=true)](https://ci.appveyor.com/project/fbarresi/beckhoffs7client)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/ce6d11762b254b939d76425339a4563b)](https://www.codacy.com/manual/fbarresi/BeckhoffS7Client?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=fbarresi/BeckhoffS7Client&amp;utm_campaign=Badge_Grade)
![Licence](https://img.shields.io/github/license/fbarresi/BeckhoffS7Client.svg)
![GitHub All Releases](https://img.shields.io/github/downloads/fbarresi/BeckhoffS7Client/total)

Unofficial TwinCAT function for S7 Communication


This software is an (unofficial) opensource implementation of [TF6610 | TC3 S5/S7 Communication](https://www.beckhoff.com.ph/default.asp?twincat/tf6760.htm) similar to my other project [BeckhoffHttpClient](https://github.com/fbarresi/BeckhoffHttpClient).

## Main features

Your surely gonna love this software, but if you still need a couple of information for starring this project...

- **FREE! (for commercial use as well)**
- Resilient: doesn't matter what you do with the beckhoff (Start, Stop, Config-Mode, Run-Mode, etc.)
- Modern: build with the newest TwinCAT Version and .Net Core 3.1
- Clever: don't need to specify any datatype or bit numbering

## Requirements

- TwinCAT 3.1.4024.7+

## How to use this software

### Install and setup connections

- [Download](https://github.com/fbarresi/BeckhoffS7Client/releases/latest) the setup and install it on your beckhoff.
- Ajdust the settings file `TFU002.settings.json` located `C:\TwinCAT\Functions\Unofficial\BeckhoffS7Client` putting your connection parameters for all your S7 PLC in the settings file. 

```json
{
  "BeckhoffSettings": {
    "AmsNetId": "",
    "Port": 851
  },
  "ExtenalPlcSettings": [
    {
      "Name": "s7-300",
      "IpAddress": "10.30.10.50",
      "Port": 102,
      "Rack": 0,
      "Slot": 2
    }
  ]
}
```

### Use The S7 Attribute

Use the S7 attribute in your project for connect your primitive variables (like an input or output) to a S7.

```reStructuredText
VAR_GLOBAL
	
	{attribute 'S7.Out'}
	{attribute 'S7.Address' := 'db2.dbx0.0'}
	imAlive : BOOL;

	{attribute 'S7.In'}
	{attribute 'S7.Address' := 'db2.dbx1.0'}
	{attribute 'S7.Plc' := 's7-300'} // you have to select a plc by name only if you have more then one
	otherSystemAlive :BOOL;
	
END_VAR
```

### Supported TwinCAT Datatypes

- BOOL
- BYTE und BYTE[]
- INT / UINT
- DINT / UDINT
- LINT / ULINT
- REAL

### S7 Addressing rules

every address has the form (case unsenible) `DB<number>.<TYPE><Start>.<Length/Position>`
i.e.: `DB42.DBX0.7` => (means) Datablock 42, Bit (DBX), Start: 0, Position: 7 .

Following types are supported:
- `DBX` => Bit (BOOL)
- `DBB` => BYTE or BYTE[]
- `INT`
- `DINT`
- `DUL` => LINT
- `D` => REAL

### Logfiles

Logfiles are saved into `C:\TwinCAT\Functions\Unofficial\BeckhoffS7Client\Service.log`. 
You can use [TailBlazer](https://github.com/RolandPheasant/TailBlazer) for a live view.


## Would you like to contribute?

Yes, please!

Try the library and feel free to open an issue or ask for support. 

Don't forget to **star this project**! 

## Credits

Special thanks to [JetBrains](https://www.jetbrains.com/?from=BeckhoffS7Client) for supporting this open source project.

<a href="https://www.jetbrains.com/?from=BeckhoffS7Client"><img height="200" src="https://www.jetbrains.com/company/brand/img/jetbrains_logo.png"></a>


