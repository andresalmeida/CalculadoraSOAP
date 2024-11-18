from flask import Flask, request, render_template
from zeep import Client

app = Flask(__name__)

# URL del WSDL
wsdl_url = "http://www.dneonline.com/calculator.asmx?WSDL"
client = Client(wsdl=wsdl_url)

@app.route('/', methods=['GET', 'POST'])
def home():
    result = None
    if request.method == 'POST':
        operation = request.form['operation']
        try:
            intA = int(request.form['intA'])
            intB = int(request.form['intB'])
            # Realizar la operación seleccionada en el cliente SOAP
            result = getattr(client.service, operation)(intA=intA, intB=intB)
        except Exception as e:
            result = f"Error: {str(e)}"

    return render_template('home.html', result=result)

if __name__ == '__main__':
    app.run(debug=True)
