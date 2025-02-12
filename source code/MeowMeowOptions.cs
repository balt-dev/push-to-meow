﻿using Menu.Remix.MixedUI;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PushToMeowMod
{
	public class MeowMeowOptions : OptionInterface
	{
		public readonly Configurable<bool> AltRivuletSounds;
		public readonly Configurable<bool> AlertCreatures;
		public readonly Configurable<bool> SpearmasterMeow;
		public readonly Configurable<bool> DrainLungs;
		public readonly Configurable<bool> CanPanicMeow;
        public readonly Configurable<float> MeowVolumeMultiplier;

		public MeowMeowOptions(PushToMeowMain plugin)
		{
			AltRivuletSounds = config.Bind("PushToMeow_AlternateRivuletSound", false);
            AlertCreatures = config.Bind("PushToMeow_AlertCreatures", true);
            SpearmasterMeow = config.Bind("PushToMeow_SpearmasterMeow", true);
            DrainLungs = config.Bind("PushToMeow_DrainLungs", true);
            CanPanicMeow = config.Bind("PushToMeow_CanPanicMeow", true);
			MeowVolumeMultiplier = config.Bind("PushToMeow_MeowVolumeMultiplier", 0.85f);
		}

		public override void Initialize()
		{
			OpTab meowTab = new OpTab(this, "Push to Meow :3");
			OpTab customMeowTab = new OpTab(this, "Custom Meows");
            Tabs = new OpTab[] { meowTab, customMeowTab };

			OpContainer tab1Container = new OpContainer(new Vector2(0, 0));
			meowTab.AddItems(tab1Container);

            OpContainer tab2Container = new OpContainer(new Vector2(0, 0));
            customMeowTab.AddItems(tab2Container);

            /*for (int i = 0; i <= 600; i += 10) // Line grid to help align things, don't leave this in your final code. Almost every element starts from bottom-left.
			{
				Color c;
				c = Color.grey;
				if (i % 50 == 0) { c = Color.yellow; }
				if (i % 100 == 0) { c = Color.red; }
				FSprite lineSprite = new FSprite("pixel");
				lineSprite.color = c;
				lineSprite.alpha = 0.2f;
				lineSprite.SetAnchor(new Vector2(0.5f, 0f));
				Vector2 a = new Vector2(i, 0);
				lineSprite.SetPosition(a);
				Vector2 b = new Vector2(i, 600);
				float rot = Custom.VecToDeg(Custom.DirVec(a, b));
				lineSprite.rotation = rot;
				lineSprite.scaleX = 2f;
				lineSprite.scaleY = Custom.Dist(a, b);
				tab1Container.container.AddChild(lineSprite);
				a = new Vector2(0, i);
				b = new Vector2(600, i);
				lineSprite = new FSprite("pixel");
				lineSprite.color = c;
				lineSprite.alpha = 0.2f;
				lineSprite.SetAnchor(new Vector2(0.5f, 0f));
				lineSprite.SetPosition(a);
				rot = Custom.VecToDeg(Custom.DirVec(a, b));
				lineSprite.rotation = rot;
				lineSprite.scaleX = 2f;
				lineSprite.scaleY = Custom.Dist(a, b);
				tab1Container.container.AddChild(lineSprite);
			}*/

			OpLabel volLabel = new OpLabel(10, 420 - 10 - 25, "Meow volume: " + (Mathf.Round(MeowVolumeMultiplier.Value * 100) + "%"), false);
			OpFloatSlider volSlider = new OpFloatSlider(MeowVolumeMultiplier, new Vector2(10 + 100 + 20, 420 - 10 - 30), 200) { description = "Changes the volume of all meows! 85% is the default :)" };
			
			volSlider.OnChange += () =>
			{
				volLabel.text = "Meow volume: " + Mathf.Round(float.Parse(volSlider.value) * 100) + "%";
			};

            UIelement[] opts = new UIelement[]
			{
				new OpLabel(new Vector2(10, 600 - 30), new Vector2(200, 30), "Push to Meow settings :3 (check out tabs for custom meows)", FLabelAlignment.Left, true) { verticalAlignment = OpLabel.LabelVAlignment.Top },
				new OpCheckBox(AltRivuletSounds, 10, 570 - 30),
				new OpLabel(45, 570 - 30 + 1, "Use alternate sounds for Rivulet (disabled = sopping wet rat, enabled = high-pitch-y sound)", false),
				new OpCheckBox(AlertCreatures, 10, 570 - 60),
				new OpLabel(45, 570 - 60 + 1, "Can meowing alert other creatures?", false),
                new OpCheckBox(SpearmasterMeow, 10, 570 - 90),
                new OpLabel(45, 570 - 90 + 1, "Can Spearmaster meow?", false),
                new OpCheckBox(DrainLungs, 10, 570 - 120),
                new OpLabel(45, 570 - 120 + 1, "Does meowing make you drown faster?", false),
                new OpCheckBox(CanPanicMeow, 10, 570 - 150) {description = "Disabling this might fix some issues that can happen, like being able to meow while you're 100% mega dead"},
                new OpLabel(45, 570 - 150 + 1, "Can slugcats panic-meow while being grabbed by lizards?", false),
				
                volSlider,
                volLabel
			};

			meowTab.AddItems(opts);


            var vsp = new OpScrollBox(customMeowTab, 2000);
            var btn = new OpSimpleButton(new Vector2(0, 0), new Vector2(70, 30), "Reload") { description = "Reloads custom meow configurations, useful if you're a mod developer making changes" };
            
			btn.OnClick += (UIfocusable e) =>
			{
				customMeowTab.RemoveItems(customMeowTab.items.ToArray());
                MeowUtils.LoadCustomMeows();

				(List<UIelement> nelist, float nvertSize) = PopulateMeowsList(btn);

                vsp = new OpScrollBox(customMeowTab, 2000) { contentSize = nvertSize };
                vsp.AddItems(nelist.ToArray());
            };

			(List<UIelement> elist, float vertSize) = PopulateMeowsList(btn);

			vsp.contentSize = vertSize;
            vsp.AddItems(elist.ToArray());
        }

		private (List<UIelement>, float vertSize) PopulateMeowsList(OpSimpleButton reloadButton)
        {
            int interval = 14;
            int intervalBetweenCMs = 30;
            int j = 0;
            float vSize = (interval * 4 + intervalBetweenCMs) * MeowUtils.CustomMeows.Count + 100;

            List<UIelement> customMeowList = new List<UIelement>
            {
                new OpLabel(10, vSize - 25, MeowUtils.CustomMeows.Count + " registered slugcat meow configurations:", true),
                new OpLabel(10, vSize - 40, "(to add your own or change existing ones, check the workshop page's custom meow guide!)") { color = new Color(0.4f, 0.4f, 0.4f) },
                reloadButton
            };

            foreach (var kv in MeowUtils.CustomMeows)
            {
				float vpos = vSize - 80 - j * (interval * 4 + intervalBetweenCMs);

                var lbID = new OpLabel(10, vpos, "ID " + kv.Key + ":");
                var lbVol = new OpLabel(140, vpos, "   VolumeMultiplier: " + kv.Value.VolumeMultiplier + "x");
                var lbNormal = new OpLabel(140, vpos - interval * 1, "   Short: " + (kv.Value.ShortMeowSoundID?.value ?? "(none)") + ", Long: " + (kv.Value.LongMeowSoundID?.value ?? "(none)"));
                var lbPup = new OpLabel(140, vpos - interval * 2, "   ShortPup: " + (kv.Value.ShortMeowPupSoundID?.value ?? "(none)") + ", LongPup: " + (kv.Value.LongMeowPupSoundID?.value ?? "(none)"));
                var lbPath = new OpLabel(140, vpos - interval * 3, "   (Hover to see JSON file path where this is defined)") { description = kv.Value.FilePath, color = new Color(0.3f, 0.3f, 0.3f) };

                j++;

                customMeowList.Add(lbID);
                customMeowList.Add(lbVol);
                customMeowList.Add(lbNormal);
                customMeowList.Add(lbPup);
                customMeowList.Add(lbPath);
            }

			return (customMeowList, vSize);
        }
	}
}
