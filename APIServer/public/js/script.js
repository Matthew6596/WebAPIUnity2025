listContainer = document.getElementById("listContainer");

const fetchPlayerData = async ()=>{
    try{
        //fetch data from server
        const response = await fetch("/player"); //get all players
        if(!response.ok){
            throw new Error("Failed to get player data");
        }

        //Parse
        const players = await response.json();

        listContainer.innerHTML = "";

        //Add to list
        players.forEach(player => {
            const listDiv = document.createElement("div");
            listDiv.className = "player";
            listDiv.innerHTML = `${player.username} ${player.besttime} ${player.wincount} ${player.gamesplayed}`;
            listContainer.appendChild(listDiv);
        });
    } catch(error){
        console.error("Error: ", error);
        listContainer.innerHTML = "<p style='color:red'>Failed to get player data</p>";
    }
}

const fetchTopTenPlayers = async ()=>{
    try{
        //fetch data from server
        const response = await fetch("/playersByWins"); //get all players sorted by wins
        if(!response.ok){
            throw new Error("Failed to get players by wins");
        }

        //Parse
        const players = await response.json();

        listContainer.innerHTML = "";

        //Add top 10 to list
        for (let i = 0; i < 10; i++){
            var player;

            if(players[i] != null){
                player = players[i];
            }
            else{
                break;
            }

            const listDiv = document.createElement("div");
            listDiv.className = "player";
            listDiv.innerHTML = `${i + 1}. ${player.username} ${player.besttime} ${player.wincount} ${player.gamesplayed}`;
            listContainer.appendChild(listDiv);
        };
    } catch(error){
        console.error("Error: ", error);
        listContainer.innerHTML = "<p style='color:red'>Failed to get player data</p>";
    }
}

//fetchPlayerData();
fetchTopTenPlayers();