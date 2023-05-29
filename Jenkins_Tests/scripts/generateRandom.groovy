import java.util.Random

def textWithNumbers(int length){
    String text = (('A'..'Z')+('a'..'z')+('0'..'9')).join() 
    def random = new Random()
    return (1..length).collect {
        text[random.nextInt(text.length())]
    }.join()
}

return this