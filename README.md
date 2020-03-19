# BeckhoffS7Client
Unofficial TwinCAT function for S7 Communication


This software is an (unofficial) opensource implementation of [TF6610 | TC3 S5/S7 Communication](https://www.beckhoff.com.ph/default.asp?twincat/tf6760.htm) similar to my other project [BeckhoffHttpClient](https://github.com/fbarresi/BeckhoffHttpClient).

## First release is coming: stay tuned!

Scheduled for April 2020


## Requirements

- .Net Core Runtime 3.1+
- TwinCAT 3.1.4024.7+

## S7 Attribute

Use the simple S7 attribute in order to mark your varible as input or output.

```reStructuredText
VAR_GLOBAL
	
	{attribute 'S7.Out'}
	{attribute 'S7.Address' := 'db2.dbx0.0'}
	imAlive : BOOL;

	{attribute 'S7.In'}
	{attribute 'S7.Address' := 'db2.dbx1.0'}
	{attribute 'S7.Plc' := 's7-300'}
	otherSystemAlive :BOOL;
	
END_VAR
```


## Settings

Setup the service settings locally on your Beckhoff or remote and support as many different S7 plc as you need.

````json
{
  "BeckhoffSettings": {
    "AmsNetId": "5.45.127.110.1.1",
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

