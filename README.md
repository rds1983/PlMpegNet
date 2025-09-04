[![NuGet](https://img.shields.io/nuget/v/PlMpegNet.svg)](https://www.nuget.org/packages/PlMpegNet/)
[![Build & Publish Beta](https://github.com/rds1983/PlMpegNet/actions/workflows/build-and-publish-beta.yml/badge.svg)](https://github.com/rds1983/PlMpegNet/actions/workflows/build-and-publish-beta.yml)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

<img width="1402" height="992" alt="image" src="https://github.com/user-attachments/assets/9bcc2565-5772-416c-b1c1-16606b1b241a" />

## PlMpegNet
PlMpegNet is C# port of the [pl_mpeg](https://github.com/phoboslab/pl_mpeg), which is C library to decode MPEG1 Video and MP2 Audio.

It is important to note, that this project is **port**(not **wrapper**). Original C code had been ported to C#. Therefore PlMpegNet doesnt require any native binaries.

The porting hasn't been done by hand, but using [Hebron](https://github.com/rds1983/Hebron), which is the C to C# code converter utility.

## Adding Reference

The library is available at NuGet: https://www.nuget.org/packages/PlMpegNet/

## Usage
Check [Samples](Samples)

Particularly [VideoPlayerWidget.cs](Samples/PlMpegNet.Samples.MonoGame.VideoPlayer/UI/VideoPlayerWidget.cs) contains code that interacts with the PlMpegNet

## Test Mpeg
https://phoboslab.org/files/bjork-all-is-full-of-love.mpg

# License
MIT

## Support
[Discord](https://discord.gg/ZeHxhCY)

## Sponsor
https://www.patreon.com/rds1983

https://boosty.to/rds1983

bitcoin: 3GeKFcv8X1cn8WqH1mr8i7jgPBkQjQuyN1
