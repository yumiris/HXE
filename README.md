# SPV3.Shaders

SPV3.Shaders is a program for SPV3.2 that handles the configuration of various post-processing graphical shader effects.
It serves as a sub-component of SPV3.Settings, though it is complex enough in specification and development to be stored
in its own dedicated repository. As such, any potential references to SPV3.Settings implicitly refer to SPV3.Shaders!

This repository serves both the purpose of storing source code revisions but also documentation on the configuration
program and all of the external shader effects that SPV3.2 uses. They include:

- MXAO, for ambient obscurance;
- DOF, for depth of field;
- Dynamic Flare, for lens flares;
- Lens Dirt, for dirt/scratch effects;
- Vignette, for darkening edge ports;
- Eye Adaptation, for adjusting scene exposure;
- SMAA, for spatial anti-aliasing;
- Debanding, for fixing banding at low bit depths;

For further documentation on the shader effects, please refer to the following documentation:

- [Shader Definitions](doc/shader-definitions.md)
- [Render Stack Sorting](doc/stack-sort.md)
- [Estimated Quality Levels](doc/quality-levels.md)

# Attributions

Although this project does not directly interact with the respective shader effects, we believe it is fair to include
the licenses for the shader effects that are distributed with the official copy of SPV3.2.

The [`docs/SHADER_LICENSES.md`](doc/SHADER_LICENSES.md) document handles the shader effects' attributions and licences.

Note that the licence used for SPV3.Shaders (or its main implementor - SPV3.Settings) does not extend to any shader
effect that is utilised by the official copy of SPV3.2. As such, the source code and/or binaries for the shader effects
are not included in the official repositories for SPV3.Shaders and SPV3.Settings.