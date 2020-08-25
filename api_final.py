import mysql.connector
from flask import Flask, request, jsonify
import uuid
from werkzeug.security import generate_password_hash, check_password_hash
import time
import socket


def connectionToDb(database=''):
    if database == '':
        db = mysql.connector.connect(
            host="localhost",
            user="root",
            passwd="1234",
            # database="wordwar"
        )

    if database != '':
         db = mysql.connector.connect(
            host="localhost",
            user="root",
            passwd="1234",
            database=database
        )

    return db


app = Flask(__name__)
# app.config['SECRET_KEY'] = 'secret'
app.config['JSON_SORT_KEYS'] = False


def checkUserForExistence(userName):
    db = connectionToDb('wordwar')
    cursor = db.cursor()
    cursor.execute(f"select user_name from USERS where user_name = '{userName}'")
    user = ''
    for x in cursor:
        user = x[0]

    if user == userName:
        return True
    else:
        return False

@app.route('/user', methods=['POST'])
def create_user():
    data = request.get_json()
    if not checkUserForExistence(data['user_name']):
        hashed_password = generate_password_hash(data['password'], method='sha256')
        # publicId = str(uuid.uuid4())
        db = connectionToDb('wordwar')
        cursor = db.cursor()
        cursor.execute(f"INSERT INTO USERS (user_name, coins, xp, weekly_rounds, password) VALUES ('{data['user_name']}', '{data['coins']}', '{data['xp']}', '{data['weekly_rounds']}', '{hashed_password}')")
        db.commit()
        return jsonify({'message': 1})
    
    else:
        return jsonify({'message': 0})

@app.route('/user-verify',  methods=['POST'])
def varifyUser():
    data = request.get_json()
    db = connectionToDb('wordwar')
    cursor = db.cursor()
    cursor.execute(f"select * from USERS where user_name = '{data['user_name']}'")
    user = {}

    for x in cursor:
        user['uID'] = x[1]
        user['coins'] = x[2]
        user['xp'] = x[3]
        user['weekly_rounds'] = x[4]
        user['password'] = x[5]

    if len(user.keys()) > 0:
        if check_password_hash(user['password'], data['password']):
            user['password'] = data['password']
            return jsonify(user)
        else:
            return jsonify({'message': 0})
    else:
        return jsonify({'message': 0})


@app.route('/update-coins', methods=['POST'])
def updateCoins():
    
    data = request.get_json()
    if checkUserForExistence(data['user_name']):
        
        db = connectionToDb('wordwar')
        cursor = db.cursor()
        cursor.execute(f"SELECT password FROM USERS where user_name = '{data['user_name']}'")
        passwd = ''

        for x in cursor:
            passwd = x[0]
            # print(passwd)
        
        if check_password_hash(passwd, data['password']):
            # print('Cleared')
            coins = 0
            cursor.execute(f"SELECT coins FROM USERS WHERE user_name = '{data['user_name']}' and password = '{passwd}'")

            for x in cursor:
                coins = int(x[0])
                # print(coins)

            if data['action'] == 'plus':
                coins += int(data['coins'])

            elif data['action'] == 'minus':
                
                if coins >= abs(int(data['coins'])):
                    coins -= abs(int(data['coins']))
            
            else:
                return jsonify({'message': 0})

            if data['action'] == 'plus':
                weekly_rounds = 0
                xp = 0
                cursor.execute(f"SELECT weekly_rounds , xp FROM USERS WHERE user_name = '{data['user_name']}' and password = '{passwd}'")
                for x in cursor:
                    print(x)
                    weekly_rounds = int(x[0])
                    xp = int(x[1])
                weekly_rounds += 1
                xp += 20
                cursor.execute(f"UPDATE USERS SET weekly_rounds = '{weekly_rounds}', xp = '{xp}'  where user_name = '{data['user_name']}' and password = '{passwd}'")
                
            cursor.execute(f"UPDATE USERS SET coins = '{coins}' where user_name = '{data['user_name']}' and password = '{passwd}'")
            db.commit()

            return jsonify({'message': 1})

        else:
            return jsonify({'message': 0})


    else:
        return jsonify({'message': 0})

@app.route('/leader-board', methods=['GET'])
def leaderBord():
    db = connectionToDb('wordwar')
    cursor = db.cursor()
    cursor.execute("SELECT user_name, coins FROM USERS ORDER BY coins DESC LIMIT 10")
    board = {}
    for x in cursor:
        board[x[0]] = x[1]
        # print(board)

    return jsonify(board)


@app.route('/tester', methods=['GET'])
def tester():
    return jsonify({'message': 'working good!'})


if __name__ == '__main__':
    app.run(debug=True)

# app.run(socket.gethostbyname(socket.gethostname()))






