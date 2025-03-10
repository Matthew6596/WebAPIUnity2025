const mongoose = require("mongoose");

const playerSchema = new mongoose.Schema({
    username:{type:String,unique:true},
    firstname:String,
    lastname:String,
    creationdate:String,
    wincount:Number,
    besttime:Number,
    gamesplayed:Number
});

const Player = mongoose.model("Player",playerSchema,"playerdata");

module.exports = Player;