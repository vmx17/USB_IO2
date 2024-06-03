# USB-IO2.0(AKI) CS
<p align="right"> The last updated 06/03/2024 by vmx17</p>  
A HID controller software for Akizuki USB-IO2.0(AKI)  

## What is this?
This is a C#-re-written [Akizuki USB-IO2.0(AKI)](https://akizukidenshi.com/catalog/g/g105131/) HID controller. The device can be used as PC's GPIO (up to 12 pins) over USB.
I made this for my personal project.

## Background and license
The original module `USB-IO2.0` is provided by [Km2Net Inc.](https://km2net.com/usb-io2.0/io_sample_kai.shtml). It has some additional features to `USB-IO2.0(AKI)`.
According to their web site, for their `USB-IO2.0`, [here the license is](https://creativecommons.org/licenses/by-sa/3.0/). It is unclear this is also effective to `USB-IO2.0(AKI)` but to make this software, I referred to their software for VendorID and ProductID. Then for the time moment, I apply same license.
The latest software for `USB-IO2.0(AKI)` was made in Windows7 era in VisualBasic. So I port it to C# using WindowsForms on .Net 8 using VisualStudio 2022.
The design and function is almost same but it is not converted by any code converter from VB to C#. All manual rework.
