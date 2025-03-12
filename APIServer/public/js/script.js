listContainer = document.getElementById("listContainer");
pageIndicator = document.getElementById("pageIndicator");

const callPageFunctions = async ()=>{
    switch (pageIndicator.title) {
        case "index":
            fetchPlayerData();
            break;
        case "mostwins":
            fetchTopTenPlayers();
            break;
        case "updateanddelete":
            fetchPlayerDataEditable();
            break;
        case "update":
            displayUpdatePage();
            break;
        default:
            break;
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

        listContainer.innerHTML = "<h3>USERNAME | BEST TIME | WINS | GAMES PLAYED<h3>";

        //Add to list
        players.forEach(player => {
            const listDiv = document.createElement("div");
            listDiv.className = "player";
            listDiv.innerHTML = `${player.username} | ${player.besttime} | ${player.wincount} | ${player.gamesplayed}`;
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

        listContainer.innerHTML = "<h3>USERNAME | BEST TIME | WINS | GAMES PLAYED<h3>";

        //Add to list
        players.forEach(player => {
            const listDiv = document.createElement("div");
            listDiv.className = "player";
            listDiv.innerHTML = `${player.username} | ${player.besttime} | ${player.wincount} | ${player.gamesplayed}
                <form action="javascript:window.location.href='/update?username=${player.username}'" method="GET"><button type="submit">Update</button></form>
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

        listContainer.innerHTML = "<h3>USERNAME | BEST TIME | WINS | GAMES PLAYED<h3>";

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
            listDiv.innerHTML = `${i + 1}. ${player.username} | ${player.besttime} | ${player.wincount} | ${player.gamesplayed}`;
            listContainer.appendChild(listDiv);
        };
    } catch(error){
        console.error("Error: ", error);
        listContainer.innerHTML = "<p style='color:red'>Failed to get player data</p>";
    }
}

const fetchPlayerDataToUpdate = async ()=>{
    try{
        const urlParams = new URLSearchParams(window.location.search);
        const response = await fetch("/player/" + urlParams.get("username"));
        if(!response.ok){
            throw new Error("Failed to get player");
        }
    
        //Parse
        const player = await response.json();
    
        return player;
    }catch(error)
    {
        console.error("Error: ", error);
        listContainer.innerHTML = "<p style='color:red'>Failed to get player</p>"
    }
}

const displayUpdatePage = async ()=>{
    //Get player to update
    const player = await fetchPlayerDataToUpdate();

    //Clear
    listContainer.innerHTML = "";

    //listContainer.className = "player";
    //PROBLEM HERE, not request.body does not contain the form data
    listContainer.innerHTML = `<form action="/update/${player.username}" method="POST"> 
        <label for="username">Username:</label><br>
        <input type="text" id="username" name="username" value="${player.username}" readonly><br>
        <label for="firstname">First Name:</label><br>
        <input type="text" id="firstname" name="firstname" value="${player.firstname}"><br>
        <label for="lastname">Last Name:</label><br>
        <input type="text" id="lastname" name="lastname" value="${player.lastname}"><br>
        <label for="creationdate">Creation Date:</label><br>
        <input type="text" id="creationdate" name="creationdate" value="${player.creationdate}"><br>
        <label for="wincount">Win Count:</label><br>
        <input type="number" id="wincount" name="wincount" value="${player.wincount}" min="0"><br>
        <label for="besttime">Best Time:</label><br>
        <input type="decimal" id="besttime" name="besttime" value="${player.besttime}" min="0"><br>
        <label for="gamesplayed">Games Played:</label><br>
        <input type="number" id="gamesplayed" name="gamesplayed" value="${player.gamesplayed}" min="0"><br>
        <button type="submit">Update</button></form>`;
    
}

callPageFunctions();