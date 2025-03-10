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
app.use(express.static("public")); //check parameters <<<

const port = 3000;

//MongoDB connection
mongoose.connect("mongodb://localhost:27017/finalsgamedb");
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
app.get("/playersByWins",async(req,res)=>{ //get all players sorted by win count
    try{
        const items = await Player.find();
        items.sort((a,b)=>b.wincount-a.wincount);
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get players."});}
});
app.get("/playersByTimes",async(req,res)=>{ //get all players sorted by best times
    try{
        const items = await Player.find();
        items.sort((a,b)=>b.besttime-a.besttime);
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get players."});}
});

/*app.get("/player/:playerid",async(req,res)=>{ //get one player based on id
    try{
        const items = await Player.findOne({"playerid":req.params.playerid});
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get player."});}
});*/

app.get("/player/:username",async(req,res)=>{ //get one player based on username
    try{
        const items = await Player.findOne({"username":req.params.username});
        res.json(items);
    }catch(e){res.status(500).json({error:"Failed to get player."});}
});

app.post("/player",async(req,res)=>{ //add new player
    try {
        let newItem = new Player(req.body);
        //Save new user
        await newItem.save();
        res.json({message:"Player Added",username:newItem.username,firstname:newItem.firstname})
    } catch (e) {res.status(501).json({error:"Failed to add new player: "+e});}

});

app.post("/player/:username",async(req,res)=>{ //update existing player
    Player.findOneAndUpdate({"username":req.params.username},req.body,{new:true,runValidators:true}).then((updatedItem)=>{
        if(!updatedItem) return res.status(404).json({error:"Player not found."});
        res.json(updatedItem);
    }).catch((e)=>{res.status(400).json({error:"Failed to update player: "+e});});
});

app.delete("/player/:username",async(req,res)=>{ //delete existing player
    Player.findOneAndDelete({"username":req.params.username},req.body,{new:true,runValidators:true}).then((updatedItem)=>{
        if(!updatedItem) return res.status(404).json({error:"Player not found."});
        res.json({"message":"Deletion successful."});
    }).catch((e)=>{res.status(400).json({error:"Failed to update player: "+e});});

});

app.listen(port,()=>{console.log(`Server running on port ${port}`)});