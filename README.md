# USB-IO2.0(AKI) CS
<p align="right"> The last updated 05/30/2024 by vmx17</p>  
A HID controller software for Akizuki USB-IO2.0(AKI)  

## What is this?
This is a C# rewritten [Akizuki USB-IO2.0(AKI)](https://akizukidenshi.com/catalog/g/g105131/) HID controller. The device can be used as GPIO (up to 12 pins) module over USB.
I made this for my personal project.

## Status
The original module `USB-IO2.0` is provided by [Km2Net Inc.](https://km2net.com/usb-io2.0/io_sample_kai.shtml). It has some additional features to `USB-IO2.0(AKI)`.
The software provided by `Km2Net Inc.`. It seems I should follow their lisence and [here is it](https://creativecommons.org/licenses/by-sa/3.0/).
The latest software for `USB-IO2.0(AKI)` was made in Windows7 era in VisualBasic. So I port it to C# using WindowsForms on .Net 8.
The design and function is almost same but it is not converted by code converter from VB to C#. All manual rework.
