import { GET_USER_FAVOURITES, USER_ERROR } from '../actions/types';

const initialState = {
	favourites: [],
	favouritesLoading: true,
	error: {}
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case GET_USER_FAVOURITES:
			return {
				...state,
				favourites: payload,
				favouritesLoading: false
			};
		case USER_ERROR:
			return {
				...state,
				error: payload,
				favouritesLoading: false
			};
		default:
			return state;
	}
}
