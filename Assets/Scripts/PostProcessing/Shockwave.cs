using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ShockwaveRenderer), PostProcessEvent.AfterStack, "Custom/Shockwave")]
public class Shockwave : PostProcessEffectSettings
{
	public Vector2Parameter center = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };
	public FloatParameter thickness = new FloatParameter { value = 0.2f };
	public FloatParameter radius = new FloatParameter { value = 1f };
}

public class ShockwaveRenderer : PostProcessEffectRenderer<Shockwave>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet sheet = context.propertySheets.Get(Shader.Find("Custom/Shockwave"));
		sheet.properties.SetFloat("_Thickness", settings.thickness);
		sheet.properties.SetFloat("_Radius", settings.radius);

		sheet.properties.SetFloat("_CenterX", settings.center.value.x);
		sheet.properties.SetFloat("_CenterY", settings.center.value.y);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
}