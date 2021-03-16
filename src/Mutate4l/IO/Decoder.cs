using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using Mutate4l.Cli;
using Mutate4l.Core;
using Mutate4l.State;
using static Mutate4l.State.InternalCommandType;

namespace Mutate4l.IO
{
    public static class Decoder
    {
        public const byte TypedDataFirstByte = 127;
        public const byte TypedDataSecondByte = 126;
        public const byte TypedDataThirdByte = 125;

        public const byte StringDataSignifier = 124;
        public const byte SetClipDataOnServerSignifier = 255;
        public const byte SetFormulaOnServerSignifier = 254;
        public const byte EvaluateFormulasSignifier = 253;
        public const byte SetAndEvaluateClipDataOnServerSignifier = 252;
        public const byte SetAndEvaluateFormulaOnServerSignifier = 251;

        public static bool IsStringData(byte[] result)
        {
            return result.Length > 4 && result[0] == TypedDataFirstByte && result[1] == TypedDataSecondByte && result[2] == TypedDataThirdByte && result[3] == StringDataSignifier;
        }

        public static bool IsTypedCommand(byte[] result)
        {
            return result.Length >= 4 && result[0] == TypedDataFirstByte && result[1] == TypedDataSecondByte && result[2] == TypedDataThirdByte;
        }

        public static InternalCommandType GetCommandType(byte dataSignifier)
        {
            return dataSignifier switch
            {
                StringDataSignifier => OutputString,
                SetClipDataOnServerSignifier => SetClipDataOnServer,
                SetFormulaOnServerSignifier => SetFormulaOnServer,
                EvaluateFormulasSignifier => EvaluateFormulas,
                SetAndEvaluateFormulaOnServerSignifier => SetAndEvaluateFormulaOnServer,
                SetAndEvaluateClipDataOnServerSignifier => SetAndEvaluateClipDataOnServer,
                _ => UnknownCommand
            };
        }

        public static (int trackNo, int clipNo, string formula) GetFormula(byte[] data)
        {
            int trackNo = data[0];
            int clipNo = data[1];
            var formula = Encoding.UTF8.GetString(data[2..]);
            return (trackNo, clipNo, formula);
        }
        
        public static string GetText(byte[] data)
        {
            if (data.Length < 5) return "";
            return Encoding.UTF8.GetString(data[4..]);
        }
        
        public static void HandleTypedCommand(byte[] data, ClipSet clipSet, ChannelWriter<InternalCommand> writer)
        {
            switch (GetCommandType(data[3]))
            {
                case OutputString:
                    CommandHandler.OutputString(data);
                    break;
                case SetFormulaOnServer:
                    CommandHandler.SetFormulaOnServer(data, clipSet);
                    break;
                case SetClipDataOnServer:
                    CommandHandler.SetClipDataOnServer(data, clipSet);
                    break;
                case EvaluateFormulas:
                    CommandHandler.EvaluateFormulas(clipSet, writer);
                    break;
                case SetAndEvaluateClipDataOnServer:
                    CommandHandler.SetAndEvaluateClipDataOnServer(data, clipSet, writer);
                    break;
                case SetAndEvaluateFormulaOnServer:
                    CommandHandler.SetAndEvaluateFormulaOnServer(data, clipSet, writer);
                    break;
                case UnknownCommand:
                    break;
            }
        }
        
        public static Clip GetSingleClip(byte[] data)
        {
            var offset = 0;
            var clipReference = new ClipReference(data[offset], data[offset += 1]);
            decimal length = (decimal)BitConverter.ToSingle(data, offset += 1);
            bool isLooping = data[offset += 4] == 1;
            var clip = new Clip(length, isLooping)
            {
                ClipReference = clipReference
            };
            ushort numNotes = BitConverter.ToUInt16(data, offset += 1);
            offset += 2;
            for (var i = 0; i < numNotes; i++)
            {
                clip.Notes.Add(new NoteEvent(
                    data[offset], 
                    (decimal)BitConverter.ToSingle(data, offset += 1), 
                    (decimal)BitConverter.ToSingle(data, offset += 4), 
                    data[offset += 4])
                );
                offset++;
            }
            return clip;
        }
        
        public static (List<Clip> Clips, string Formula, ushort Id, byte TrackNo) DecodeData(byte[] data)
        {
            var clips = new List<Clip>();
            ushort id = BitConverter.ToUInt16(data, 0);
            byte trackNo = data[2];
            byte numClips = data[3];
            int dataOffset = 4;

            // Decode clipdata
            while (clips.Count < numClips)
            {
                ClipReference clipReference = new ClipReference(data[dataOffset], data[dataOffset += 1]);
                decimal length = (decimal)BitConverter.ToSingle(data, dataOffset += 1);
                bool isLooping = data[dataOffset += 4] == 1;
                var clip = new Clip(length, isLooping) {
                    ClipReference = clipReference
                };
                ushort numNotes = BitConverter.ToUInt16(data, dataOffset += 1);
                dataOffset += 2;
                for (var i = 0; i < numNotes; i++)
                {
                    clip.Notes.Add(new NoteEvent(
                        data[dataOffset], 
                        (decimal)BitConverter.ToSingle(data, dataOffset += 1), 
                        (decimal)BitConverter.ToSingle(data, dataOffset += 4), 
                        data[dataOffset += 4])
                    );
                    dataOffset++;
                }
                clips.Add(clip);
            }
            // Convert remaining bytes to text containing the formula
            string formula = Encoding.ASCII.GetString(data, dataOffset, data.Length - dataOffset);

            return (clips, formula, id, trackNo);
        }
    }
}