listContainer = document.getElementById("listContainer");
pageIndicator = document.getElementById("pageIndicator");

const callPageFunctions = async ()=>{
    if(pageIndicator.title == "index"){
        fetchPlayerData();
    }
    else if(pageIndicator.title == "mostwins"){
        fetchTopTenPlayers();
    }
    else if(pageIndicator.title == "updateanddelete"){
        fetchPlayerDataEditable();
    }
}

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

const fetchPlayerDataEditable = async ()=>{
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
            listDiv.innerHTML = `${player.username} ${player.besttime} ${player.wincount} ${player.gamesplayed}
                <form action="javascript:window.location.href='/update.html?id=${player.username}'" method="GET"><button type="submit">Update</button></form>
                <form action="/delete/${player.username}" method="POST"><button type="submit">Delete</button></form>`;
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

callPageFunctions();