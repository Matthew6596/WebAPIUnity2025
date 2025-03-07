const express = require("express");
const mongoose = require("mongoose");
const bodyParser = require("body-parser");
const fs = require("fs");
const cors = require("cors");
const {nanoid} = require("nanoid");
const Player = require("./models/Player");

const app = express();
app.use(express.json());
app.use(cors()); //allows making requests from game
app.use(bodyParser.json());

const port = 3000;

//MongoDB connection
mongoose.connect("mongodb://localhost:27017/gamedb");
const db = mongoose.connection;

db.on("error",console.error.bind(console,"MongoDB connection error"));
db.once("open",()=>{console.log("Connected to mongodb");});

//API routes
app.get("/player",async(req,res)=>{ //get all players
    try{
        const items = await Player.find();
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get players."});}
});

app.get("/player/:playerid",async(req,res)=>{ //get one player based on id
    try{
        const items = await Player.findOne({"playerid":req.params.playerid});
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get player."});}
});

app.get("/playerByName/:username",async(req,res)=>{ //get one player based on id
    try{
        const items = await Player.findOne({"username":req.params.username});
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get player."});}
});

app.post("/player",async(req,res)=>{ //add new player
    try {
        let newItem = new Player(req.body);
        newItem.playerid = nanoid(8);
        //Save new user
        await newItem.save();
        res.json({message:"Player Added",playerid:newItem.playerid,name:newItem.name})
    } catch (e) {res.status(501).json({error:"Failed to add new player: "+e});}

});

app.post("/player/:playerid",async(req,res)=>{ //update existing player
    Player.findOneAndUpdate({"playerid":req.params.playerid},req.body,{new:true,runValidators:true}).then((updatedItem)=>{
        if(!updatedItem) return res.status(404).json({error:"Player not found."});
        res.json(updatedItem);
    }).catch((e)=>{res.status(400).json({error:"Failed to update player: "+e});});

});

app.delete("/player/:playerid",async(req,res)=>{ //delete existing player
    Player.findOneAndDelete({"playerid":req.params.playerid},req.body,{new:true,runValidators:true}).then((updatedItem)=>{
        if(!updatedItem) return res.status(404).json({error:"Player not found."});
        res.json({"message":"Deletion successful."});
    }).catch((e)=>{res.status(400).json({error:"Failed to update player: "+e});});

});

app.listen(port,()=>{console.log(`Server running on port ${port}`)});