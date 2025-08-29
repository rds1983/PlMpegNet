[![NuGet](https://img.shields.io/nuget/v/PlMpegNet.svg)](https://www.nuget.org/packages/PlMpegNet/)
[![Build & Publish Beta](https://github.com/rds1983/PlMpegNet/actions/workflows/build-and-publish-beta.yml/badge.svg)](https://github.com/rds1983/PlMpegNet/actions/workflows/build-and-publish-beta.yml)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

# PlMpegNet
PlMpegNet is C# port of the [pl_mpeg](https://github.com/phoboslab/pl_mpeg), which is C library to decode MPEG1 Video and MP2 Audio.

It is important to note, that this project is **port**(not **wrapper**). Original C code had been ported to C#. Therefore PlMpegNet doesnt require any native binaries.

The porting hasn't been done by hand, but using [Hebron](https://github.com/rds1983/Hebron), which is the C to C# code converter utility.

# Usage
Check [Samples](Samples)

Particularly [VideoPlayerWidget.cs](Samples/PlMpegNet.Samples.VideoPlayer/UI/VideoPlayerWidget.cs) contains code that interacts with the PlMpegNet

## Test Mpeg
If you just want to quickly test the library, try this file:

https://phoboslab.org/files/bjork-all-is-full-of-love.mpg
