using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;
using System.Threading.Tasks;

namespace ToDo
{
	public partial class MasterViewController : UITableViewController
	{
		DataSource dataSource;

		public MasterViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("All Tasks", "All Tasks");
		}

		void AddNewItem (object sender, EventArgs args)
		{
			dataSource.Tasks.Insert (0, new TrelloCard());

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
			
			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource ((authenticationViewController) => {
				InvokeOnMainThread (() =>
					this.PresentViewController (authenticationViewController, true, null));
			}, 
				() => {
					InvokeOnMainThread (() => TableView.ReloadData ());
				}, 
				(indexPaths) => {
					InvokeOnMainThread (() =>
						TableView.DeleteRows (indexPaths, UITableViewRowAnimation.Fade));
				});
		}

		class DataSource : UITableViewSource
		{
			static readonly NSString CellIdentifier = new NSString ("Cell");
			public List<TrelloCard> Tasks = new List<TrelloCard>();
			public List<TrelloBoard> Boards = new List<TrelloBoard>();
			TrelloCardRepository cardRepo;
			TrelloBoardRepository boardRepo;
			TrelloListRepository trelloListRepository;
			Action _refreshTable;
			Action<NSIndexPath[]> _deleteRows;

			public DataSource (Action<UIViewController> presentAuthenticationController, Action refreshTable, Action<NSIndexPath[]> deleteRows)
			{
				cardRepo = new TrelloCardRepository(presentAuthenticationController);
				boardRepo = new TrelloBoardRepository(presentAuthenticationController);
				trelloListRepository = new TrelloListRepository(presentAuthenticationController);
				_refreshTable = refreshTable;
				_deleteRows = deleteRows;
				FetchData();
			}

			private void ReceivedData(Task<List<TrelloCard>> task, Task<List<TrelloBoard>> boards){
				Tasks = task.Result;
				Boards = boards.Result;
				_refreshTable ();
			}

			private void FetchData(){
				var cards = cardRepo.AllTasks ();
				/*var boards = boardRepo.GetAllBoards ();
				Task.WaitAll (boards, cards);*/
				//ReceivedData (cards, boards);


				cards.ContinueWith ((result) => {
					Tasks = result.Result;
					var alot = Tasks.Select(t => t.ListId).Distinct().AsParallel().Select(async a => await trelloListRepository.GetById(a));

					_refreshTable ();
				});

			}

			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return Tasks.Count;
			}
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (UITableViewCell)tableView.DequeueReusableCell (CellIdentifier, indexPath);

				TrelloCard item = Tasks [indexPath.Row];
				cell.TextLabel.Text = item.Name;

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
					Tasks.RemoveAt (indexPath.Row);
					_deleteRows (new NSIndexPath[] { indexPath });
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
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail") {
				var indexPath = TableView.IndexPathForSelectedRow;
				var item = dataSource.Tasks [indexPath.Row];

				((DetailViewController)segue.DestinationViewController).SetDetailItem (item);
			}
		}
	}
}

