### Native Texture

`native-texture` is a Unity package that provides Burst/job-friendly native containers over texture memory with direct pointer access. It enables high‑performance CPU-side texture generation, processing, and efficient upload to Unity `Texture2D` or GPU buffers without unnecessary copies.

- **Key types**
  - `NativeTexture2D<T>`, `NativeTexture3D<T>`: native containers over pixel memory; can wrap an existing `Texture2D` or own a raw buffer.
  - `INativeTexture<TCoord, TValue>`: common interface for resolution, indexing, and `NativeArray` interop.
  - `UnsafeTexture2D<T>`: minimal-overhead variant with direct pointer access.
  - Utilities: `MipUtility`, `NativeTextureUnsafeUtility`, `IndexCoordUtils`, `TextureBoundsUtility`.
  - Extensions: `ReadPixel`, `ReadPixelBilinear`, `Contains`, `Swap` across formats like `byte2/3/4`, `ushort2/3/4`, `float`.

### Why it matters
- **Performance**: Native containers with `GetUnsafePtr()`/`AsArray()` interop are Burst/jobs compatible and avoid extra copies (especially when wrapping `Texture2D` data).
- **Interoperability**: Bridges CPU-generated content (noise, procedural images) to `Texture2D`/`GraphicsBuffer` via `ApplyTo(Texture2D)` and `SetData(NativeArray<T>)`.

### Installation
This repository embeds `native-texture` as a Git submodule at `Packages/native-texture`.
- Package name: `native-texture`
- Depends on: `com.unity.collections` (2.6.3), `com.unity.mathematics` (1.3.3)
- External usage: add the Git URL `https://github.com/api-haus/native-texture.git`

### Core API (brief)
- Construction
  - Wrap an existing Unity texture: `new NativeTexture2D<T>(Texture2D unityTex)`
  - Own a raw buffer: `new NativeTexture2D<T>(int2 resolution, int mipCount, TextureFormat fmt, Allocator alloc)`
  - Convert an existing pointer: `NativeTextureUnsafeUtility.ConvertExistingDataToNativeTexture2D<T>(void* ptr, int2 res, int mipCount, Allocator alloc)`
- Access
  - Indexers: `texture[int index]`, `texture[int2 coord]`
  - `AsArray()`: returns `NativeArray<T>` over the pixel buffer
  - `GetUnsafePtr()` / `GetUnsafeReadOnlyPtr()`
- Upload
  - `ApplyTo(Texture2D texture, bool updateMipmaps = false)`
- Utilities
  - `MipUtility.MipCount(int2 res)`, `TexelLength(res, mipCount)`
- Extensions
  - `ReadPixel` / `ReadPixelBilinear`, `Contains(coord)`, `Swap(i1, i2)`

### Usage examples
- Wrap a Unity texture and write a pixel, then apply back:
```csharp
using NativeTexture;
using Unity.Mathematics;
using UnityEngine;

var w = 256;
var h = 256;
var unityTex = new Texture2D(w, h, TextureFormat.RGBAFloat, false, true);
var pixels = new NativeTexture2D<float4>(unityTex);
var center = new int2(w / 2, h / 2);
pixels[center] = new float4(1, 0, 0, 1);
pixels.ApplyTo(unityTex, updateMipmaps: false);
```

- Allocate a CPU-owned buffer and upload to a `Texture2D`:
```csharp
using NativeTexture;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

int w = 256, h = 256;
var pixels = new NativeTexture2D<float>(new int2(w, h), mipCount: 1, TextureFormat.RFloat, Allocator.Persistent);
for (int y = 0; y < h; y++)
for (int x = 0; x < w; x++)
    pixels[new int2(x, y)] = math.saturate(math.sin(x * 0.05f) * math.cos(y * 0.05f));

var target = new Texture2D(w, h, TextureFormat.RFloat, false, true);
pixels.ApplyTo(target);
pixels.Dispose();
```

- Bilinear sampling in normalized UV space (float texture):
```csharp
using NativeTexture;
using NativeTexture.Extensions;
using Unity.Mathematics;

NativeTexture2D<float> tex = /* created elsewhere */;
float sample = tex.ReadPixelBilinear(new float2(0.3f, 0.6f));
```

### Notes
- `T` must be unmanaged. Use a matching `TextureFormat` for correct `sizeof(T)` mapping.
- When wrapping an existing `Texture2D`, memory ownership remains with Unity; dispose only buffers you own.
- Burst/jobs ready: safety handles and min/max index constraints; `Dispose(JobHandle)` is available for scheduled cleanup.

### Source
- Submodule path: `Packages/native-texture`
- Assembly: `NativeTexture` (`Packages/native-texture/Runtime/NativeTexture.asmdef`)
