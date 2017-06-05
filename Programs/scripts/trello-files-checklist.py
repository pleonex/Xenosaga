# Copyright 2017 Benito Palacios (aka pleonex)
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#     http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agrleed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
"""Create a checklist with file name entries per folder."""
from __future__ import print_function
from argparse import ArgumentParser
from os import environ, listdir
from os.path import basename, isfile, join
from trello import TrelloClient


def ask_arguments():
    """Ask for the command-line arguments."""
    parser = ArgumentParser('Create Trello checklist with files for cards')
    parser.add_argument("card",
                        help="The ID of the card to add the checklist")
    parser.add_argument("folder",
                        help="Folder with files for the checklist")
    return parser.parse_args()


def main():
    """Main application method."""
    # Check for keys
    trello_key = environ.get('TRELLO_KEY')
    trello_token = environ.get('TRELLO_TOKEN')
    if not trello_key:
        print("ERROR: Missing environment variable TRELLO_KEY")
        return
    if not trello_token:
        print("ERROR: Missing environment variable TRELLO_TOKEN")
        return

    # Get arguments
    args = ask_arguments()

    # Create client
    print("Connecting to Trello")
    client = TrelloClient(api_key=trello_key, token=trello_token)

    folder = basename(args.folder)
    files = [f for f in listdir(args.folder) if isfile(join(args.folder, f))]
    print("Creating checklist for %d files" % len(files))
    client.get_card(args.card).add_checklist(folder, sorted(files))


if __name__ == "__main__":
    main()
