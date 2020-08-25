#word_json 

import json

def main():

    obj = {
        "words":[]
    }

    with open("medi_words.json", 'w') as words_file:

        with open("eng.txt", 'r') as file:
            for i in file.readlines():
                if i != "\n":
                    if i.split("\n")[0] not in obj["words"]:
                        obj["words"].append(i.split(" ")[0].strip("\n"))

            json.dump(obj, words_file)

        print("Done")


if __name__ == "__main__":
    main()