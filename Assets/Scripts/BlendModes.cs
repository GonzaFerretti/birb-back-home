using UnityEngine;
using System.Collections;

using UnityEngine.Rendering;

[ExecuteInEditMode]
public class BlendModes : MonoBehaviour
{
    public enum BlendModeOption
    {
        Blend,          // SrcColor * SrcAlpha + DstColor * OneMinusSrcAlpha
        Darken,         // Min(SrcColor, DstColor)                              : SrcColor = lerp(1, SrcColor, SrcAlpha)
        Multiply,       // SrcColor * DstColor + DstColor * OneMinusSrcAlpha    : SrcColor = SrcColor * SrcAlpha
        ColorBurn,      // SrcColor + DstColor * (1 - SrcColor)                 : SrcColor = 1 - (1 / SrcColor)
        LinearBurn,     // DstColor + SrcColor - 1                              : SrcColor = (SrcColor - 1.0) * SrcAlpha
        Lighten,        // Max(SrcColor, DstColor)                              : SrcColor = lerp(0, SrcColor, SrcAlpha)
        Screen,         // SrcColor + DstColor * OneMinusSrcColor               : SrcColor = SrcColor * SrcAlpha
        ColorDodge,     // 0 + DstColor * SrcColor                              : SrcColor = (1 / (1 - SrcColor * SrcAlpha));
        LinearDodge,    // SrcColor * SrcAlpha + DstColor                       : SrcColor = SrcColor
        Overlay,
        // Overlay requires quite a lengthy explanation. We start off with the original formula
        //
        // 2 * Dst * Src,                       Dst < 0.5
        // 1 - 2 * (1 - Dst) * (1 - Src)        Dst > 0.5
        // 
        // Because there are no conditionals in the blend units, an approximation would be to linearly
        // blend. The resulting formula after simplification is
        //
        // (4 * Src - 1) * Dst + (2 - 4 * Src) * Dst * Dst
        //
        // The only way to get Dst * Dst is to isolate the term and do it in two passes, therefore we divide
        // and end up with the following formula:
        //
        // [(4 * Src - 1) * Dst / (2 - 4 * Src) +  Dst * Dst ] * (2 - 4 * Src)
        //
        // This formula does not respect the background color. What we need is to linearly interpolate this
        // big formula with alpha, where an alpha of 0 returns Dst. Therefore
        //
        // [(4 * Src - 1) * Dst / (2 - 4 * Src) +  Dst * Dst ] * (2 - 4 * Src) * a + Dst * (1 - a)
        //
        // If we include the last term into the original formula, we can still do it in 2 passes. We need to
        // be careful to clamp the alpha value with max(0.001, a) because we're now potentially dividing by 0.
        // The final formula is
        //
        // K_1 = (4 * Src - 1) / (2 - 4 * Src)
        // K_2 = (1 - a) / ((2 - 4 * Src) * a)
        //
        // [Dst * [K_1 + K_2] +  Dst * Dst ] * (2 - 4 * Src) * a
        SoftLight,      // Same as overlay but main formula after simplification is 2 * Src * Dst + (1 - 2 * Src) * Dst * Dst
                        // Source: Pegtop's formula
        HardLight,
        VividLight,
        LinearLight,
        //PinLight, // Seems impossible to implement because it uses min and max
        //Difference,
        //Exclusion,
        //Luminosity

    }

    public BlendModeOption blendModeOption;
    public Shader shader;
    public Renderer currentRenderer;

    // Use this for initialization
    void Start ()
    {
        currentRenderer = GetComponent<Renderer>();
    }

    void SetBlendModeHint(Material mat, BlendModeOption blendMode)
    {
        mat.SetFloat("_BlendMode", (float) blendMode);
    }

    void SetBlendOperation1(Material mat, BlendOp blendOp, BlendMode blendSrc, BlendMode blendDst)
    {
        SetBlendOperation1(mat, blendOp, blendSrc, blendDst, blendSrc, blendDst);
    }

    void SetBlendOperation1(Material mat, BlendOp colorBlendOp, BlendMode blendSrc, BlendMode blendDst, BlendMode alphaBlendSrc, BlendMode alphaBlendDst)
    {
        mat.SetFloat("_BlendOp1", (float) colorBlendOp);
        mat.SetFloat("_BlendSrc1", (float) blendSrc);
        mat.SetFloat("_BlendDst1", (float) blendDst);
        mat.SetFloat("_BlendSrcAlpha1", (float) alphaBlendSrc);
        mat.SetFloat("_BlendDstAlpha1", (float) alphaBlendDst);
    }

    void SetBlendOperation2(Material mat, BlendOp blendOp, BlendMode blendSrc, BlendMode blendDst)
    {
        SetBlendOperation2(mat, blendOp, blendSrc, blendDst, blendSrc, blendDst);
    }

    void SetBlendOperation2(Material mat, BlendOp colorBlendOp, BlendMode blendSrc, BlendMode blendDst, BlendMode alphaBlendSrc, BlendMode alphaBlendDst)
    {
        mat.SetFloat("_BlendOp2", (float)colorBlendOp);
        mat.SetFloat("_BlendSrc2", (float)blendSrc);
        mat.SetFloat("_BlendDst2", (float)blendDst);
        mat.SetFloat("_BlendSrcAlpha2", (float)alphaBlendSrc);
        mat.SetFloat("_BlendDstAlpha2", (float)alphaBlendDst);
    }

    // Update is called once per frame
    void Update ()
    {
        Material mat = currentRenderer.material;     
        if(mat)
        {
            mat.shader = shader;

            SetBlendOperation2(mat, BlendOp.Add, BlendMode.Zero, BlendMode.One); // Defaults to background passthrough

            switch (blendModeOption)
            {
                case BlendModeOption.Blend:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
                    break;
                case BlendModeOption.Darken:
                    SetBlendOperation1(mat, BlendOp.Min, BlendMode.One, BlendMode.One);
                    break;
                case BlendModeOption.Multiply:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.OneMinusSrcAlpha);
                    break;
                case BlendModeOption.ColorBurn:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.One, BlendMode.OneMinusSrcColor);
                    break;
                case BlendModeOption.LinearBurn:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.One, BlendMode.One);
                    break;
                case BlendModeOption.Lighten:
                    SetBlendOperation1(mat, BlendOp.Max, BlendMode.One, BlendMode.One);
                    break;
                case BlendModeOption.Screen:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.One, BlendMode.OneMinusSrcColor);
                    break;
                case BlendModeOption.ColorDodge:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.Zero);
                    break;
                case BlendModeOption.LinearDodge:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.SrcAlpha, BlendMode.One);
                    break;
                case BlendModeOption.Overlay:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.DstColor);
                    SetBlendOperation2(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.Zero);
                    break;
                case BlendModeOption.SoftLight:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.DstColor);
                    SetBlendOperation2(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.Zero);
                    break;
                case BlendModeOption.HardLight:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.One, BlendMode.One);
                    SetBlendOperation2(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.Zero);
                    break;
                case BlendModeOption.VividLight:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.DstColor, BlendMode.Zero);
                    SetBlendOperation2(mat, BlendOp.Add, BlendMode.One, BlendMode.OneMinusSrcColor);
                    break;
                case BlendModeOption.LinearLight:
                    SetBlendOperation1(mat, BlendOp.Add, BlendMode.One, BlendMode.One);
                    //SetBlendOperation2(mat, BlendOp.Add, BlendMode.One, BlendMode.SrcColor);
                    break;
            }

            SetBlendModeHint(mat, blendModeOption);

            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    }
}
