using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using ToDo.Core.iOS;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ToDo.UI.iPhone
{
	public partial class MasterViewController : UITableViewController
	{
		DataSource dataSource;

		public MasterViewController () : base ("MasterViewController", null)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Master", "Master");
			// Custom initialization
		}

		public DetailViewController DetailViewController {
			get;
			set;
		}

		void AddNewItem (object sender, EventArgs args)
		{
			dataSource.Items.Insert (0, new TodoItem{
				Title = "New item"
			});

			using (var indexPath = NSIndexPath.FromRowSection (0, 0))
				TableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var authenticatorFactory = new TrelloAuthenticatiorFactory ((vc) => 
				InvokeOnMainThread (() =>
					this.PresentViewController(vc, true, null)
				));

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource (this, authenticatorFactory);
		}

		class DataSource : UITableViewSource
		{
			static readonly NSString CellIdentifier = new NSString ("Cell");
			readonly MasterViewController controller;
			TrelloRepositoryFactory repoFactory;

			public ObservableCollection<TodoItem> Items {get;set;}

			public DataSource (MasterViewController controller, IAuthenticatorFactory authenticatorFactory)
			{
				this.controller = controller;
				Items = new ObservableCollection<TodoItem>();
				Initialize(authenticatorFactory);
			}

			public async void Initialize(IAuthenticatorFactory authenticatorFactory){
				var authenticator = await authenticatorFactory.GetAuthenticatior();
				repoFactory = new TrelloRepositoryFactory (authenticator);
				LoadData ();
			}

			public async void LoadData(){
				var repo = repoFactory.GetInstance<TodoItemRepository> ();
				var tasks = await repo.GetAllForCurrentUser();
				Items = new ObservableCollection<TodoItem> (tasks);
				Items.CollectionChanged += async (object sender, NotifyCollectionChangedEventArgs e) => {
					var oldArray = new TodoItem[e.OldItems.Count];
					e.OldItems.CopyTo(oldArray, 0);
					await repoFactory.GetInstance<TodoItemRepository>().Remove(oldArray);

					var newArray = new TodoItem[e.NewItems.Count];
					e.NewItems.CopyTo(newArray, 0);
					await repoFactory.GetInstance<TodoItemRepository>().Add(newArray);
				};
				InvokeOnMainThread (() => {
					controller.TableView.ReloadData();
				});
			}

			public IEnumerable<TodoItem> Objects {
				get { return Items; }
			}
			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return Items.Count;
			}
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (CellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}

				var item = Items [indexPath.Row] as TodoItem;

				cell.TextLabel.Text = item.Title;

				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					// Delete the row from the data source.
					Items.RemoveAt (indexPath.Row);
					controller.TableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				} else if (editingStyle == UITableViewCellEditingStyle.Insert) {
					// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
				}
			}
			/*
			// Override to support rearranging the table view.
			public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
			{
			}
			*/
			/*
			// Override to support conditional rearranging of the table view.
			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the item to be re-orderable.
				return true;
			}
			*/
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (controller.DetailViewController == null)
					controller.DetailViewController = new DetailViewController ();

				controller.DetailViewController.SetDetailItem (Items [indexPath.Row]);

				// Pass the selected object to the new view controller.
				controller.NavigationController.PushViewController (controller.DetailViewController, true);
			}
		}
	}
}
