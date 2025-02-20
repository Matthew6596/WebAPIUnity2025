const mongoose = require("mongoose");

const playerSchema = new mongoose.Schema({
    playerid:{type:String,unique:true},
    username:{type:String,unique:true},
    firstname:String,
    lastname:String,
    creationdate:String,
    score:Number
});

const Player = mongoose.model("Player",playerSchema,"playerdata");

module.exports = Player;