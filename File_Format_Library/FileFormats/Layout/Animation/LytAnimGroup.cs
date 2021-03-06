﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Animations;

namespace LayoutBXLYT
{
    public interface IAnimationTarget
    {
        LytAnimTrack GetTrack(int target);
    }

    /// <summary>
    /// Layout animation group that stores multiple tag entries.
    /// These map to either pane or materials
    /// </summary>
    public class LytAnimGroup : STAnimGroup
    {
        public BxlanPaiEntry animEntry;

        public AnimationTarget Target
        {
            get { return animEntry.Target;  }
        }

        public void RemoveKey(Type groupType)
        {
            var group = SearchGroup(groupType);
            if (group == null) return;


        }

        public void InsertKey(Type groupType)
        {
            var group = SearchGroup(groupType);
            string platform = "F";
            if (animEntry is BFLAN.PaiEntry)

            //First we find the proper group to insert our key
            //If it doesn't exist, create it.
            if (groupType == typeof(LytPaneSRTGroup))
            {
                if (group == null) {
                    var tag = new BxlanPaiTag($"{platform}LPA");
                    group = new LytPaneSRTGroup(tag);
                    animEntry.Tags.Add(tag);
                    SubAnimGroups.Add(group);
                }


            }
        }

        public STAnimGroup SearchGroup(Type groupType)
        {
            for (int i = 0; i < SubAnimGroups.Count; i++)
            {
                if (SubAnimGroups[i].GetType() == groupType)
                    return SubAnimGroups[i];
            }
            return null;
        }

        public LytAnimGroup(BxlanPaiEntry entry)
        {
            animEntry = entry;
            Name = entry.Name;
            if (entry.Target == AnimationTarget.Material)
                Category = "Materials";
            else if (entry.Target == AnimationTarget.Pane)
                Category = "Panes";
            else
                Category = "User Data";

            //Generate sub groups which contain the track data
            for (int i = 0; i < entry.Tags?.Count; i++)
            {
                STAnimGroup group = new STAnimGroup();
                string tag = entry.Tags[i].Tag.Remove(0,1);
                switch (tag)
                {
                    case "LPA":
                        group = new LytPaneSRTGroup(entry.Tags[i]);
                        break;
                    case "LVI":
                        group = new LytVisibiltyGroup(entry.Tags[i]);
                        break;
                    case "LTS":
                        group = new LytTextureSRTGroup(entry.Tags[i]);
                        break;
                    case "LVC":
                        group = new LytVertexColorGroup(entry.Tags[i]);
                        break;
                    case "LMC":
                        group = new LytMaterialColorGroup(entry.Tags[i]);
                        break;
                    case "LIM":
                        group = new LytIndirectSRTGroup(entry.Tags[i]);
                        break;
                    case "LTP":
                        group = new LytTexturePatternGroup(entry.Tags[i]);
                        break;
                    case "LAC":
                        group = new LytAlphaTestGroup(entry.Tags[i]);
                        break;
                    case "LCT":
                        group = new LytFontShadowGroup(entry.Tags[i]);
                        break;
                    case "LCC":
                        group = new LytPerCharacterTransformCurveGroup(entry.Tags[i]);
                        break;
                }

                foreach (var keyGroup in entry.Tags[i].Entries)
                {
                    if (!(group is IAnimationTarget))
                        continue;

                    var targetGroup = ((IAnimationTarget)group).GetTrack(keyGroup.AnimationTarget);
                    if (targetGroup != null)
                    {
                        targetGroup.LoadKeyFrames(keyGroup.KeyFrames);
                        targetGroup.Name = keyGroup.TargetName;

                        if (keyGroup.CurveType == CurveType.Constant)
                            targetGroup.InterpolationType = STInterpoaltionType.Constant;
                        else if (keyGroup.CurveType == CurveType.Hermite)
                            targetGroup.InterpolationType = STInterpoaltionType.Hermite;
                        else if (keyGroup.CurveType == CurveType.Step)
                            targetGroup.InterpolationType = STInterpoaltionType.Step;
                    }
                    else
                        Console.WriteLine($"Unsupported track type for tag {keyGroup.TargetName} {keyGroup.AnimationTarget}");
                }

                group.Name = entry.Tags[i].Type;
                SubAnimGroups.Add(group);
            }
        }
    }

    public class LytAlphaTestGroup : STAnimGroup
    {
        public LytAlphaTestGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytFontShadowGroup : STAnimGroup
    {
        public LytFontShadowGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytPerCharacterTransformCurveGroup : STAnimGroup
    {
        public LytPerCharacterTransformCurveGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytPaneSRTGroup : STAnimGroup, IAnimationTarget
    {
        public BxlanPaiTag Tag;

        public LytAnimTrack TranslateX = new LytAnimTrack();
        public LytAnimTrack TranslateY = new LytAnimTrack();
        public LytAnimTrack TranslateZ = new LytAnimTrack();

        public LytAnimTrack RotateX = new LytAnimTrack();
        public LytAnimTrack RotateY = new LytAnimTrack();
        public LytAnimTrack RotateZ = new LytAnimTrack();

        public LytAnimTrack ScaleX = new LytAnimTrack();
        public LytAnimTrack ScaleY = new LytAnimTrack();

        public LytAnimTrack SizeX = new LytAnimTrack();
        public LytAnimTrack SizeY = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 10; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target)
        {
            switch (target)
            {
                case 0: return TranslateX;
                case 1: return TranslateY;
                case 2: return TranslateZ;
                case 3: return RotateX;
                case 4: return RotateY;
                case 5: return RotateZ;
                case 6: return ScaleX;
                case 7: return ScaleY;
                case 8: return SizeX;
                case 9: return SizeY;
                default: return null;
            }
        }

        public LytPaneSRTGroup(BxlanPaiTag entry)
        {
            Tag = entry;
        }
    }

    public class LytTexturePatternGroup : STAnimGroup, IAnimationTarget
    {
        public LytAnimTrack Tex0AnimTrack = new LytAnimTrack();
        public LytAnimTrack Tex1AnimTrack = new LytAnimTrack();
        public LytAnimTrack Tex2AnimTrack = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 3; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target) {
            if (target == 0)
                return Tex0AnimTrack;
            else if (target == 1)
                return Tex1AnimTrack;
            else if (target == 2)
                return Tex2AnimTrack;
            else
                return Tex0AnimTrack;
        }

        public LytTexturePatternGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytVisibiltyGroup : STAnimGroup, IAnimationTarget
    {
        public LytAnimTrack AnimTrack = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 1; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target) {
            return AnimTrack;
        }

        public LytVisibiltyGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytVertexColorGroup : STAnimGroup, IAnimationTarget
    {
        public LytAnimTrack TopLeftR = new LytAnimTrack();
        public LytAnimTrack TopLeftG = new LytAnimTrack();
        public LytAnimTrack TopLeftB = new LytAnimTrack();
        public LytAnimTrack TopLeftA = new LytAnimTrack();

        public LytAnimTrack TopRightR = new LytAnimTrack();
        public LytAnimTrack TopRightG = new LytAnimTrack();
        public LytAnimTrack TopRightB = new LytAnimTrack();
        public LytAnimTrack TopRightA = new LytAnimTrack();

        public LytAnimTrack BottomLeftR = new LytAnimTrack();
        public LytAnimTrack BottomLeftG = new LytAnimTrack();
        public LytAnimTrack BottomLeftB = new LytAnimTrack();
        public LytAnimTrack BottomLeftA = new LytAnimTrack();

        public LytAnimTrack BottomRightR = new LytAnimTrack();
        public LytAnimTrack BottomRightG = new LytAnimTrack();
        public LytAnimTrack BottomRightB = new LytAnimTrack();
        public LytAnimTrack BottomRightA = new LytAnimTrack();

        public LytAnimTrack Alpha = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 17; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target)
        {
            switch (target) {
                case 0: return TopLeftR;
                case 1: return TopLeftG;
                case 2: return TopLeftB;
                case 3: return TopLeftA;
                case 4: return TopRightR;
                case 5: return TopRightG;
                case 6: return TopRightB;
                case 7: return TopRightA;
                case 8: return BottomLeftR;
                case 9: return BottomLeftG;
                case 10: return BottomLeftB;
                case 11: return BottomLeftA;
                case 12: return BottomRightR;
                case 13: return BottomRightG;
                case 14: return BottomRightB;
                case 15: return BottomRightA;
                case 16: return Alpha;
                default: return null;
            }
        }

        public LytVertexColorGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytMaterialColorGroup : STAnimGroup, IAnimationTarget
    {
        public LytAnimTrack BlackColorR = new LytAnimTrack();
        public LytAnimTrack BlackColorG = new LytAnimTrack();
        public LytAnimTrack BlackColorB = new LytAnimTrack();
        public LytAnimTrack BlackColorA = new LytAnimTrack();

        public LytAnimTrack WhiteColorR = new LytAnimTrack();
        public LytAnimTrack WhiteColorG = new LytAnimTrack();
        public LytAnimTrack WhiteColorB = new LytAnimTrack();
        public LytAnimTrack WhiteColorA = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 8; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target)
        {
            switch (target)
            {
                case 0: return BlackColorR;
                case 1: return BlackColorG;
                case 2: return BlackColorB;
                case 3: return BlackColorA;
                case 4: return WhiteColorR;
                case 5: return WhiteColorG;
                case 6: return WhiteColorB;
                case 7: return WhiteColorA;
                default: return null;
            }
        }

        public LytMaterialColorGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytTextureSRTGroup : STAnimGroup, IAnimationTarget
    {
        public LytAnimTrack TranslateU = new LytAnimTrack();
        public LytAnimTrack TranslateV = new LytAnimTrack();
        public LytAnimTrack Rotate = new LytAnimTrack();
        public LytAnimTrack ScaleU = new LytAnimTrack();
        public LytAnimTrack ScaleV = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 5; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target)
        {
            switch (target)
            {
                case 0: return TranslateU;
                case 1: return TranslateV;
                case 2: return Rotate;
                case 3: return ScaleU;
                case 4: return ScaleV;
                default: return null;
            }
        }

        public LytTextureSRTGroup(BxlanPaiTag entry)
        {

        }
    }

    public class LytIndirectSRTGroup : STAnimGroup, IAnimationTarget
    {
        public LytAnimTrack Rotate = new LytAnimTrack();
        public LytAnimTrack ScaleU = new LytAnimTrack();
        public LytAnimTrack ScaleV = new LytAnimTrack();

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            for (int i = 0; i < 3; i++)
                tracks.Add(GetTrack(i));
            return tracks;
        }

        public LytAnimTrack GetTrack(int target)
        {
            switch (target)
            {
                case 0: return Rotate;
                case 1: return ScaleU;
                case 2: return ScaleV;
                default: return null;
            }
        }

        public LytIndirectSRTGroup(BxlanPaiTag entry)
        {

        }
    }
}
