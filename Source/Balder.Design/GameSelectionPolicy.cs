using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;

namespace Balder.Design
{
	public class GameSelectionPolicy : PrimarySelectionPolicy
	{
		private bool _isFocused;

		protected override bool IsInPolicy(Selection selection, ModelItem item)
		{
			bool inPolicy = base.IsInPolicy(selection, item);
			return inPolicy && !_isFocused;
		}

		protected override void OnActivated()
		{
			Context.Items.Subscribe<FocusedTask>(OnFocusedTaskChanged);
			base.OnActivated();
		}

		protected override void OnDeactivated()
		{
			this.Context.Items.Unsubscribe<FocusedTask>(OnFocusedTaskChanged);
			base.OnDeactivated();
		}

		private void OnFocusedTaskChanged(FocusedTask f)
		{
			bool nowFocused = f.Task != null;
			if (nowFocused != _isFocused)
			{
				_isFocused = nowFocused;

				Selection selection = Context.Items.GetValue<Selection>();
				if (selection.PrimarySelection != null)
				{
					ModelItem[] removed;
					ModelItem[] added;

					if (nowFocused)
					{
						removed = new ModelItem[] { selection.PrimarySelection };
						added = new ModelItem[0];
					}
					else
					{
						removed = new ModelItem[0];
						added = new ModelItem[] { selection.PrimarySelection };
					}

					OnPolicyItemsChanged(new PolicyItemsChangedEventArgs(this, added, removed));
				}
			}
		}
	}
}