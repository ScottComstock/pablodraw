using System;
using Eto.Drawing;

namespace Pablo.Formats.Character.Undo
{
	public class UndoDeleteColumn : UndoBuffer
	{
		public int Column { get; set; }
		
		public override int UndoID {
			get { return (int)UndoType.UndoDeleteColumn; }
		}
		
		public UndoDeleteColumn ()
		{
		}
		
		public UndoDeleteColumn (CharacterHandler handler, int column)
		{
			this.Column = column;
			var canvas = handler.CurrentPage.Canvas;
			Add (new UndoRect (handler, new Rectangle (Column, 0, 1, canvas.Height)));
		}

		public override IUndoItem Reciprocal (CharacterHandler handler)
		{
			return new UndoInsertColumn (handler, this.Column){ CursorLocation = this.CursorLocation };
		}
		
		public override void Apply (CharacterHandler handler)
		{
			var canvas = handler.CurrentPage.Canvas;
			canvas.InsertColumn (Column);
			base.Apply (handler);
			handler.InvalidateCharacterRegion (new Rectangle (Column, 0, canvas.Width - Column, canvas.Height), true);
		}
		
		public override bool Send (Pablo.Network.SendCommandArgs args)
		{
			base.Send (args);
			args.Message.WriteVariableInt32 (Column);
			return true;
		}
		
		public override void Receive (Pablo.Network.ReceiveCommandArgs args)
		{
			base.Receive (args);
			Column = args.Message.ReadVariableInt32 ();
		}
	}
}

