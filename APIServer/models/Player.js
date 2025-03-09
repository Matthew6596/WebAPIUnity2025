const mongoose = require("mongoose");

const playerSchema = new mongoose.Schema({
    username:{type:String,unique:true},
    firstname:String,
    lastname:String,
    creationdate:String,
    winCount:Number,
    bestTime:String
});

const Player = mongoose.model("Player",playerSchema,"playerdata");

module.exports = Player;